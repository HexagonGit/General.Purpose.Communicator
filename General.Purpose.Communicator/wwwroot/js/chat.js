"use strict";
let token;      // токен
let username;   // имя пользователя

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub", { accessTokenFactory: () => token }).build();

//Disable send button until connection is established


connection.on("ReceiveMessage", function (username, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = username + " " + msg;
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("w3review").value = document.getElementById("w3review").value + "\n" + li.textContent;
    document.getElementById("w3review").scrollTop = document.getElementById("w3review").scrollHeight;
});

connection.on("SetOnlineUsers", function (message) {
    var encodedMsg = username;
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("onlineBlock").innerHTML = message;
});

document.getElementById("loginBtn").addEventListener("click", function (e) {

    var request = new XMLHttpRequest();
    // посылаем запрос на адрес "/token", в ответ получим токен и имя пользователя
    request.open("POST", "/token", true);
    request.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
    request.addEventListener("load", function () {
        if (request.status < 400) { // если запрос успешный

            let data = JSON.parse(request.response);   // парсим ответ  
            token = data.access_token;
            username = data.username;



            connection.start()       // начинаем соединение с хабом
                .catch(err => {
                    console.error(err.toString());
                    document.getElementById("loginBtn").disabled = true;
                });


                var element = document.getElementById("loginBlock");
                element.parentNode.removeChild(element);
            }
        
    });
    // отправляем запрос на аутентификацию
    request.send("username=" + document.getElementById("userName").value +
        "&password=" + document.getElementById("userPassword").value);
});

/*
connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});
*/



document.getElementById("messageInput").addEventListener("keyup", function (event) {
    // Number 13 is the "Enter" key on the keyboard
    if (event.keyCode === 13) {
        // Cancel the default action, if needed
        event.preventDefault();
        // Trigger the button element with a click
        var message = document.getElementById("messageInput").value;
        connection.invoke("SendMessage", username, message).catch(function (err) {
            return console.error(err.toString());
        });
        document.getElementById("messageInput").value = "";
    }
});

//Prevent Select of messageInput box from mouse
document.getElementById("messageInput").addEventListener('select', function () {
    this.selectionStart = this.selectionEnd;
}, false);