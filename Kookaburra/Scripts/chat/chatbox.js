
var chatbox =
    '<div id="onlinechatwrapper" style="position: fixed; bottom: 0px; right: 20px; margin: 0px; padding: 0px; overflow: hidden; z-index: 99999999; width: 300px; height: 330px; background-color: transparent;">' +
    '<iframe id="onlinechatiframe" scrolling="0" frameborder="0" src="http://secure.onlinechat.com/chatbox" ' +
        'style="background-color: transparent;vertical-align: text-bottom; overflow: hidden; position: relative; width: 100%; height:100%; margin: 0px; z-index: 999999;">' +
    '</iframe></div>';
 
document.body.innerHTML += chatbox;