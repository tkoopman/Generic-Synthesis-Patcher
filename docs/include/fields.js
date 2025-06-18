'use strict';

let gridApi;
let dataTypes;
let game;

function getQueryParams(qs) {
    qs = qs.split('+').join(' ');

    var params = {},
        tokens,
        re = /[?&]?([^=]+)=([^&]*)/g;

    while (tokens = re.exec(qs)) {
        params[decodeURIComponent(tokens[1])] = decodeURIComponent(tokens[2]);
    }

    return params;
}

function loadTypes(rt) {
    const g = game.value;
    if (typeof rt === 'undefined')
        rt = dataTypes.value;

    dataTypes.options.length = 0;

    let index = 0;

    fetch('./data/' + g + '/types.json')
        .then(res => res.json())
        .then(data => {
            data.forEach(type => {
                let option = new Option(type.name, type.id);
                if (type.id == rt) {
                    index = dataTypes.options.length
                }
                dataTypes.add(option);
            });
        })
        .then(() => {
            dataTypes.selectedIndex = index
        })
        .then(() => loadType());
}

function loadType() {
    let g = game.options[game.selectedIndex].getAttribute('value');
    let rt = dataTypes.value;
    let name = dataTypes.options[dataTypes.selectedIndex].innerHTML.replace(/\s/g, '');
    if (name.toLowerCase() == rt.toLowerCase()) {
        name = rt;
    } else {
        name = rt + '; ' + name;
    }
    document.getElementById('typeAlias').innerHTML = name;

    if (window.location.search != '' || dataTypes.selectedIndex != 0 || g != 'Skyrim')
        queryString.push({ 'game': g, 'record': rt });

    fetch('./data/' + g + '/' + rt.toLowerCase() + '.json')
        .then((response) => response.json())
        .then((data) => gridApi.setGridOption('rowData', data))
        .then(() => gridApi.autoSizeColumns([]));
}

function flagImg(value, flag, img, title) {
    const flagElement = document.createElement('img');
    flagElement.src = 'img/' + img;

    if ((value & flag) == flag) {
        flagElement.setAttribute('class', 'flagSVG flag-enabled');
    } else {
        flagElement.setAttribute('class', 'flagSVG flag-disabled');
    }

    flagElement.setAttribute('title', title);

    return flagElement;
}

function flagRenderer(params) {
    const flagSpan = document.createElement('span');
    flagSpan.setAttribute('class', 'imgSpan');

    // CanMatch
    flagSpan.appendChild(flagImg(params.value, 4, 'canmatch-svgrepo-com.svg', 'Match'));

    // CanFill
    flagSpan.appendChild(flagImg(params.value, 8, 'canfill-svgrepo-com.svg', 'Fill'));

    // CanFoward
    flagSpan.appendChild(flagImg(params.value, 16, 'canforward-svgrepo-com.svg', 'Forward'));

    // CanFowardSelf
    flagSpan.appendChild(flagImg(params.value, 32, 'canforwardself-svgrepo-com.svg', 'Forward only entries itself created'));

    // CanMerge
    flagSpan.appendChild(flagImg(params.value, 64, 'canmerge-svgrepo-com.svg', 'Merge property across multiple record instances'));

    return flagSpan;
}

function onLoad() {
    game = document.getElementById('game');
    dataTypes = document.getElementById('dataTypes');

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
            { field: 'Name' },
            { field: 'Aliases' },
            { field: 'Flags', headerName: 'Valid Uses', cellRenderer: flagRenderer, suppressHeaderFilterButton: true, sortable: false, width: 120 },
            { field: 'Description', wrapText: true, autoHeight: true, flex: 1 },
            { field: 'Example', wrapText: true, autoHeight: true, flex: 1 },
        ],
        autoSizeStrategy: {
            type: 'fitCellContents',
            skipHeader: false,
            colIds: ['Name', 'Aliases'],
        },
        rowSelection: {
            mode: 'singleRow',
            checkboxes: false,
            enableClickSelection: true,
        },
    };

    gridApi = agGrid.createGrid(document.querySelector('#myGrid'), gridOptions);

    const q = getQueryParams(window.location.search);
    let g = q.game;
    if (typeof g === 'undefined') g = 'Skyrim';
    game.value = g;
    if (typeof q.record !== 'undefined') dataTypes.value = q.record;

    loadTypes(q.record);

    addEventListener('popstate', function (e) {
        const q = getQueryParams(window.location.search);
        let g = q.game;
        if (typeof g === 'undefined') {
            g = 'Skyrim';
        }
        const gameChanged = game.value != g;
        game.value = g;
        if (typeof q.record !== 'undefined')
            dataTypes.value = q.record;
        else
            dataTypes.selectedIndex = 0;

        if (gameChanged) {
            loadTypes(q.record);
        } else {
            loadType();
        }
    });
}

/*!
query-string
Parse and stringify URL query strings
https://github.com/sindresorhus/query-string
by Sindre Sorhus
MIT License
*/
(function () {
    'use strict';
    var queryString = {};

    queryString.parse = function (str) {
        if (typeof str !== 'string') {
            return {};
        }

        str = str.trim().replace(/^\?/, '');

        if (!str) {
            return {};
        }

        return str.trim().split('&').reduce(function (ret, param) {
            var parts = param.replace(/\+/g, ' ').split('=');
            var key = parts[0];
            var val = parts[1];

            key = decodeURIComponent(key);

            // missing `=` should be `null`:
            // http://w3.org/TR/2012/WD-url-20120524/#collect-url-parameters
            val = val === undefined ? null : decodeURIComponent(val);

            if (!ret.hasOwnProperty(key)) {
                ret[key] = val;
            } else if (Array.isArray(ret[key])) {
                ret[key].push(val);
            } else {
                ret[key] = [ret[key], val];
            }

            return ret;
        }, {});
    };

    queryString.stringify = function (obj) {
        return obj ? Object.keys(obj).map(function (key) {
            var val = obj[key];

            if (Array.isArray(val)) {
                return val.map(function (val2) {
                    return encodeURIComponent(key) + '=' + encodeURIComponent(val2);
                }).join('&');
            }

            return encodeURIComponent(key) + '=' + encodeURIComponent(val);
        }).join('&') : '';
    };

    queryString.push = function (setParams) {
        var params = queryString.parse(location.search);
        let changed = false;
        for (var key in setParams) {
            if (params[key] != setParams[key]) {
                changed = true;
                params[key] = setParams[key];
            }
        }

        if (changed) {
            var new_params_string = queryString.stringify(params)
            history.pushState({}, '', window.location.pathname + '?' + new_params_string);
        }
    }

    if (typeof module !== 'undefined' && module.exports) {
        module.exports = queryString;
    } else {
        window.queryString = queryString;
    }
})();