namespace Yoyo.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Models;
using Umbraco.Cms.Core.Web;

[Route("umbraco/api/[controller]")]
[ApiController]
public class RickAndMortyDashboardController(IContentService contentService) : ControllerBase
{
    private static List<Character> characters { get; set; } = [];
    private static DateTime lastRetrievalDateTime { get; set; }

    [HttpGet("ImportRickAndMortyCharacters")]
    public async Task<ActionResult<List<Character>>?> ImportRickAndMortyCharactersAsync()
    {
        if (characters.Count > 0 && lastRetrievalDateTime > DateTime.Now.AddHours(-.5))
        { return characters; }

        try
        {
            await MakeApiCall();
            lastRetrievalDateTime = DateTime.Now;
        }
        catch (Exception)
        {
            characters = [];
            return BadRequest("Failed to import Rick and Morty characters.");
        }
        return characters;
    }

    [HttpPost("CreateCharacterContent/{id:int}")]
    public IActionResult CreateCharacterContent(int id)
    {
        if (id <= 0 || id > characters.Count) return BadRequest("Character ID not recognized");

        var character = characters[--id];

        var homepage = contentService.GetRootContent().FirstOrDefault();
        var children = contentService.GetPagedChildren(homepage!.Id, 0, int.MaxValue, out _);

        var existingContent = children.FirstOrDefault(x => x.Name == $"{character.id}-{character.name}");
        
        var characterContent = existingContent is null 
            ? contentService.Create($"{character.id}-{character.name}", homepage?.Id ?? -1, "CharacterPage")
            : contentService.GetById(existingContent.Id);

        characterContent.SetValue("characterName", character.name);
        characterContent.SetValue("status", character.status);
        characterContent.SetValue("species", character.species);
        characterContent.SetValue("gender", character.gender);
        characterContent.SetValue("image", character.image);
        characterContent.SetValue("type", character.type);
        characterContent.SetValue("origin", character.origin?.name);
        characterContent.SetValue("location", character.location?.name);
        
        // Save and publish the content
        contentService.SaveAndPublish(characterContent);

        return Content("Character created successfully.");
    }

    private static async Task MakeApiCall(string url = "https://rickandmortyapi.com/api/character")
    {
        var client = new HttpClient();
        while (true)
        {
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var characterResponse = JsonSerializer.Deserialize<CharacterResponse>(responseBody)
                                    ?? throw new Exception("Could not deserialize response");

            if (characterResponse.results is not null) characters.AddRange(characterResponse.results);
            if (characterResponse.info?.next is not null)
            {
                url = characterResponse.info.next;
                continue;
            }

            break;
        }
    }

}