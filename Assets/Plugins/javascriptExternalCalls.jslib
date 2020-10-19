mergeInto(LibraryManager.library, {

    Hello: function () {
        window.alert("Hello, world!");
    },

    CancelLoadingData: function () {
        // Create a new event
        var event = new CustomEvent('CancelLoadingData');

        // Dispatch the event
        window.dispatchEvent(event);
        },

});