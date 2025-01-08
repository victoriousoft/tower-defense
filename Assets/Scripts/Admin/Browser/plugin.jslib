mergeInto(LibraryManager.library, {
	SendMessageToJS: function (data) {
		var message = UTF8ToString(data);

		window.parent.postMessage(
			{
				type: "unityToJs",
				data: message,
			},
			"*"
		);
	},

	InitMessageListener: function () {
		if (typeof window !== "undefined") {
			window.addEventListener("message", function (event) {
				if (event.data.type !== "jsToUnity") return;
				SendMessage("BrowserMessanger", "ReceiveFromJavaScript", event.data.data);
			});
		}
	},
});
