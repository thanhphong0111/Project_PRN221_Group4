// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/commentHub").build();
document.getElementById("sendButton").disabled = true;

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("ReceivedMess", function (username, message, time) {
    var anime__review__item = document.createElement("div");

    // create picture html dom
    var anime__review__item__pic = document.createElement("div");

    anime__review__item__pic.className = 'anime__review__item__pic'
    var img = document.createElement("img");

    img.src = 'img/anime/review-6.jpg';
    img.alt = '';

    anime__review__item__pic.appendChild(img);


    // create comment html dom
    var anime__review__item__text = document.createElement("div");

    anime__review__item__text.className = 'anime__review__item__text'

    var h6 = document.createElement("h6");

    h6.innerHTML = username + ' - <span>' + time + '</span>';

    var p = document.createElement("p");

    p.innerText = message;

    anime__review__item__text.appendChild(h6);
    anime__review__item__text.appendChild(p);

    anime__review__item.appendChild(anime__review__item__pic);
    anime__review__item.appendChild(anime__review__item__text);

    document.getElementById('anime__details__review').appendChild(anime__review__item);

});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var userId = document.getElementById("userId").value;
    var gameId = document.getElementById("gameId").value;
    var message = document.getElementById("messageInput").value;

    document.getElementById("messageInput").value = '';
    connection.invoke("PostComment", userId, gameId, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});
