function success(position) {
    var url = $('#URL_RecordLocation').val();
    var data = JSON.stringify([
        {
            id: $('#UID').val(),
            deviceid: 'web',
            latitude: position.coords.latitude,
            longitude: position.coords.longitude,
        },
    ]);

    $.ajax({
        url: url,
        data: data,
        method: 'POST',
        contentType: 'application/json; charset=utf-8',
        cache: false,
        success: function (response) {
            for (const res of response) {
                console.log(`location logged: ${[res.Latitude, res.Longitude]}`);
            }
        },
        error: function (xhr) {
            console.error(xhr.responseText);
        }
    });
}

function error(err) {
    console.error('Sorry, no position available');
}

const options = {
    enableHighAccuracy: true,
    maximumAge: 30000,
    timeout: 27000
};

$(() => {
    let nav = 'geolocation' in navigator;
    let uid = $("#UID").val() != '';

    if (!nav) {
        console.error('Geolocation services not available');
    } else if (!uid) {
        console.error('User not signed in, not checking location');
    } else {
        const watchID = navigator.geolocation.watchPosition(success, error, options);
        console.log(`Starting location monitoring... (id:${watchID})`);
    }
});

