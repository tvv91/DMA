let connection = new signalR.HubConnectionBuilder().withUrl("/defaulthub").build();

// connecting to signalr hub
async function start() {
    try {
        await connection.start();
    } catch (err) {
        console.log(err);
    }
};