$.sqlinjection.util.sendRequest = (function() {
  "use strict";


  $.ajax({
    url: "demo_test.txt",
    success: function(result) {
      return new { status: "success", result: result };
    },
    error: function(errorMessage) {
      return new { status: "error", error: errorMessage };
    }
  });

});