let loadingElement = document.getElementById("loading");
let spinner = document.getElementById("spinner");
let characterDropdown = document.getElementById('characterDropdown');
let importButton = document.getElementById("importButton");
let createButton = document.getElementById("createButton");
let characterDetails = document.getElementById("characterDetails");
let searchInput = document.getElementById("searchInput");
var charactersData = [];

createButton.addEventListener('click', function () {
    var selectedCharacterId = characterDropdown.value;

    if (selectedCharacterId) {
        fetch('/umbraco/api/RickAndMortyDashboard/CreateCharacterContent/' + selectedCharacterId, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            }
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Failed to create character.');
                }
            })
            .then(_ => { location.reload(); })
            .catch(error => {
                console.error('Error:', error);
                alert('Error creating character.');
            });
    } else {
        alert('Please select a character before creating.');
    }

});
searchInput.addEventListener('input', function () {
    var searchValue = searchInput.value.toLowerCase();
    var filteredCharacters = charactersData.filter(function (character) {
        return character.name.toLowerCase().includes(searchValue);
    });
    updateDropdownOptions(filteredCharacters);
});

function updateDropdownOptions(data) {
    characterDropdown.innerHTML = '';
    var defaultOption = document.createElement('option');
    defaultOption.text = 'Select a character';
    characterDropdown.add(defaultOption);
    for (var i = 0; i < data.length; i++) {
        var character = data[i];
        var option = document.createElement('option');
        option.value = character.id;
        option.text = character.name;
        characterDropdown.add(option);
    }
}

importButton.addEventListener('click', function () {

    loadingElement.style.display = "block";
    spinner.style.display = "block";
    importButton.style.display = "none";

    fetch('/umbraco/api/RickAndMortyDashboard/ImportRickAndMortyCharacters', {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json'
        }
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Failed to fetch data from the API.');
            }
            createButton.style.display = "none";
            loadingElement.style.display = "none";
            spinner.style.display = "none";
            importButton.style.display = "block";
            characterDropdown.style.display = "none";
            searchInput.style.display = "none";
            return response.json();
        })
        .then(data => {
            charactersData = data;
            updateDropdownOptions(data);
            characterDropdown.innerHTML = '';

            var defaultOption = document.createElement('option');
            defaultOption.text = 'Select a character';
            characterDropdown.add(defaultOption);

            for (var i = 0; i < data.length; i++) {
                var character = data[i];

                var option = document.createElement('option');
                option.value = character.id;
                option.text = character.name;
                characterDropdown.add(option);
            }

            characterDropdown.addEventListener('change', function () {
                characterDetails.style.display = "block";
                var selectedCharacterId = characterDropdown.value;

                var selectedCharacterData = data.find(function (character) {
                    return character.id.toString() === selectedCharacterId;
                });

                characterDetails.innerHTML = '<h3>' + selectedCharacterData.name + '</h3>' +
                    '<img src="' + selectedCharacterData.image + '" alt="Image of ' + selectedCharacterData.name + '">' +
                    '<ul>' +
                    '<li><b>Status:</b> ' + selectedCharacterData.status + '</li>' +
                    '<li><b>Species:</b> ' + selectedCharacterData.species + '</li>' +
                    (selectedCharacterData.type ? '<li><b>Type:</b> ' + selectedCharacterData.type + '</li>' : '') +
                    '<li><b>Gender:</b> ' + selectedCharacterData.gender + '</li>' +
                    '<li><b>Origin:</b> ' + selectedCharacterData.origin.name + '</li>' +
                    '<li><b>Location:</b> ' + selectedCharacterData.location.name + '</li>' +
                    '</ul>';

            });

            createButton.style.display = "block";
            loadingElement.style.display = "none";
            spinner.style.display = "none";
            importButton.style.display = "none";
            characterDropdown.style.display = "block";
            searchInput.style.display = "block";
        })
        .catch(error => {
            console.error('Error:', error);
            alert('Error importing data.');

            createButton.style.display = "none";
            loadingElement.style.display = "none";
            spinner.style.display = "none";
            importButton.style.display = "block";
            characterDropdown.style.display = "none";
            searchInput.style.display = "none";
        });
})