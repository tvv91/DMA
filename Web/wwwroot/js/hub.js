let connection = new signalR.HubConnectionBuilder().withUrl("/defaulthub").build();
// for store album id's
let albumIds = [];



// connecting to signalr hub
async function start() {
    try {
        await connection.start();
    } catch (err) {
        console.log(err);
    }
};

// pass album id's to hub
async function getAlbumCovers() {
    try {
        await connection.invoke("GetAlbumCovers", connection.connectionId, albumIds);
    } catch (err) {
        console.log("Error sending request to GetAlbumCovers");
    }
}

// SignalR handlers
connection.onclose(async () => {
    await start();
});

// change img src and stop loading animation
connection.on("ReceivedAlbumConver", (albumId, cover) => {
    $(`img[id=${albumId}]`).attr("src", cover);
    $(`img[id=${albumId}]`).attr("class", "");
});

function onAlbumLoad(albumId) {
    albumIds.push(albumId);
}

async function getTechnicalInfoIcons(albumId) {
    try {
        await connection.invoke("GetTechnicalInfoIcons", connection.connectionId, albumId);
    } catch (err) {
        console.log("Error sending request to GetTechnicalInfoIcons");
    }
}

connection.on("TechnicalInfoNotFound", () => {
    $('#noinfowarning').fadeIn(3000);
});

connection.on("TechnicalInfoFound", () => {
    $('#techinfoicons').fadeIn(500);   
});

connection.on("ReceivedTechnicalInfoIcon", (category, cover) => {
    console.log(category, cover);
    $(`#${category}div`).fadeIn(1000);
    window.setTimeout(() => {
        $(`div[id=${category}div]`).attr("class", "");
        $(`img[id=${category}]`).css("display", "");
    }, 1000);
    $(`img[id=${category}]`).attr("src", cover);
});

