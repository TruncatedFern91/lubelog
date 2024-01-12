function showAddVehicleModal() {
    uploadedFile = "";
    $.get('/Vehicle/AddVehiclePartialView', function (data) {
        if (data) {
            $("#addVehicleModalContent").html(data);
        }
    })
    $('#addVehicleModal').modal('show');
}
function hideAddVehicleModal() {
    $('#addVehicleModal').modal('hide');
}
//refreshable function to reload Garage PartialView
function loadGarage() {
    $.get('/Home/Garage', function (data) {
        $("#garageContainer").html(data);
    });
}
function loadSettings() {
    $.get('/Home/Settings', function (data) {
        $("#settings-tab-pane").html(data);
    });
}
function loadUsers() {
    $.get('/Login/GetUsers', function (data) {
        $("#user-tab-pane").html(data);
    })
}
function showAddUser() {
    Swal.fire({
        title: 'Setup Credentials',
        html: `
                        <input type="text" id="authUsername" class="swal2-input" placeholder="Username">
                        <input type="password" id="authPassword" class="swal2-input" placeholder="Password">
                        `,
        confirmButtonText: 'Setup',
        focusConfirm: false,
        preConfirm: () => {
            const username = $("#authUsername").val();
            const password = $("#authPassword").val();
            if (!username || !password) {
                Swal.showValidationMessage(`Please enter username and password`)
            }
            return { username, password }
        },
    }).then(function (result) {
        if (result.isConfirmed) {
            $.post('/Login/CreateUser', { userName: result.value.username, password: result.value.password }, function (data) {
                if (data) {
                    loadUsers();
                } else {
                    errorToast("An error occurred, please try again later.");
                }
            });
        };
    });
}
function performLogOut() {
    $.post('/Login/LogOut', function (data) {
        if (data) {
            window.location.href = '/Login';
        }
    })
}