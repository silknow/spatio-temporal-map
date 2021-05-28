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
    DataLoaded: function () {
            OnDataLoaded();
            },
     WriteToConsole: function (text) {
           var para = document.createElement("P");              
           para.innerText = Pointer_stringify(text);
           $( "#evaluationConsole" ).append( para );
           },

});