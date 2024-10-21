mergeInto(LibraryManager.library, {
	SendMessageToJS: function (str) {
		var message = UTF8ToString(str);
		console.log("Message from Unity: " + message);

		window.parent.postMessage(
			{
				type: "unityMessage",
				data: message,
			},
			"*"
		);
	},

	InitMessageListener: function () {
		if (typeof window !== "undefined") {
			window.addEventListener("message", function (event) {
				console.log("PLUGIN - Message from JavaScript: " + event.data.data);
				SendMessage("BrowserMessanger", "ReceiveFromJavaScript", event.data.data);
			});
		}
	},
});
