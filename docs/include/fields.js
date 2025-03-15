let gridApi;
let dataTypes;

function loadTypes() {
    fetch("./data/types.json")
        .then(res => res.json())
        .then(data => {
            data.forEach(type => {
                let option = new Option(type.name, type.id);
                dataTypes.add(option);
            });
        })
        .then(() => dataTypes.selectedIndex = 0)
        .then(() => loadType());
}

function loadType() {
    let value = dataTypes.options[dataTypes.selectedIndex].getAttribute('value');
    let name = dataTypes.options[dataTypes.selectedIndex].innerHTML.replace(/\s/g, '');
    if (name.toLowerCase() == value.toLowerCase()) {
        name = value;
    } else {
        name = value + "; " + name;
    }
    document.getElementById("typeAlias").innerHTML = name;
    fetch("./data/" + value.toLowerCase() + ".json")
        .then((response) => response.json())
        .then((data) => gridApi.setGridOption("rowData", data))
        .then(() => gridApi.autoSizeColumns(["Name", "Aliases", "MFFSM"]));
}

function onLoad() {
    dataTypes = document.getElementById("dataTypes");

    const myTheme = agGrid.themeQuartz.withPart(agGrid.colorSchemeDarkBlue)
        .withParams({
            headerBackgroundColor: '#111',
            backgroundColor: '#333',
            oddRowBackgroundColor: '#444',
            foregroundColor: '#ddd',
            headerTextColor: '#04AA6D',
            accentColor: '#04AA6D',
        });

    const gridOptions = {
        theme: myTheme,
        rowData: [],
        columnDefs: [
            { field: "Name" },
            { field: "Aliases" },
            { field: "MFFSM" },
            { field: "Description", wrapText: true, autoHeight: true, flex: 1 },
            {
                field: "Example", wrapText: true, autoHeight: true, flex: 1,
                cellRenderer: (params) => {
                    return params.value;
                },
            },
        ],
        autoSizeStrategy: {
            type: "fitCellContents",
            colIds: ["name", "Aliases", "MFFSM"],
        },
        enableCellTextSelection: true,
        ensureDomOrder: true,
    };

    gridApi = agGrid.createGrid(document.querySelector("#myGrid"), gridOptions);

    loadTypes();
}