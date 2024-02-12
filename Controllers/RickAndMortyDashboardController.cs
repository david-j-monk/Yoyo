namespace Yoyo.Controllers;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using System.Text.Json;
using Yoyo.Models;
using Umbraco.Extensions;

[Route("umbraco/api/[controller]")]
[ApiController]
public class RickAndMortyDashboardController : ControllerBase
{
    private readonly IContentService _contentService;
    private static List<Character> Characters { get; set; } = [];
    public RickAndMortyDashboardController(IContentService contentService)
    {
        _contentService = contentService;
    }

    [HttpPost("ImportRickAndMortyCharacters")]
    public async Task<ActionResult<List<Character>>?> ImportRickAndMortyCharactersAsync()
    {
        if (Characters.Count > 0) return Characters;
        try
        {
            await MakeApiCall();
        }
        catch (Exception)
        {
            Characters = new List<Character>();
            return BadRequest("Failed to import Rick and Morty characters.");
        }
        return Characters;
    }

    // POST action to handle form submission
    [HttpPost("CreateCharacter/{id}")]
    public IActionResult CreateCharacter(int id)
    {
            var character = Characters[--id];
            // Create Umbraco content node
            IContent characterContent = _contentService.Create(character.name, -1, "CharacterPage");
            characterContent.SetValue("characterName", character.name);
            characterContent.SetValue("status", character.status);
            characterContent.SetValue("species", character.species);
            characterContent.SetValue("gender", character.gender);

            // Save and publish the content
            _contentService.SaveAndPublish(characterContent);

            return Content("Character created successfully.");


        // If ModelState is not valid, handle accordingly
        return BadRequest("Invalid model state");
    }

    private async Task MakeApiCall(string url = "https://rickandmortyapi.com/api/character")
    {
        var client = new HttpClient();
        while (true)
        {
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var characterResponse = JsonSerializer.Deserialize<CharacterResponse>(responseBody);
            Characters.AddRange(characterResponse.results);
            if (characterResponse.info.next != null)
            {
                url = characterResponse.info.next;
                continue;
            }

            break;
        }
    }

}