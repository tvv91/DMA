// Create connections to all hubs
let albumConnection = new signalR.HubConnectionBuilder().withUrl("/albumhub").build();
let equipmentConnection = new signalR.HubConnectionBuilder().withUrl("/equipmenthub").build();
let postConnection = new signalR.HubConnectionBuilder().withUrl("/posthub").build();

// For backward compatibility, keep the default connection
let connection = albumConnection;

// connecting to all signalr hubs
async function start() {
    try {
        await Promise.all([
            albumConnection.start(),
            equipmentConnection.start(),
            postConnection.start()
        ]);
    } catch (err) {
        console.log(err);
    }
};