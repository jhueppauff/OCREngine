const serverUrl = "http://localhost:7071/api";
var requestId;

function QueueDocument(){
    var settings = {
        "async": true,
        "crossDomain": true,
        "url": ""+ serverUrl +"/api/QueueDocument",
        "method": "PUT",
        "headers": {
          "Content-Type": "application/json",
          "cache-control": "no-cache"
        },
        "processData": false,
        "data": "{\n\t\"DownloadUrl\" : \""+  $('#url').val + "\" }"
      };
      
      $.ajax(settings).done(function (response) {
        console.log(response);
      });
}

function CheckResponse(){

}
