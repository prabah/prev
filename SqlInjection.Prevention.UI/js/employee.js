(function () {
    "use strict";
    var writingTextUri = "http://localhost/SqlInjection.Prevention.UI/Default/WriteQuery";
    var checkFileExistsUri = "http://localhost/SqlInjection.Prevention.API/api/filemanager/fileexists/";
    var writeDataToFileUri = "http://localhost/SqlInjection.Prevention.API/api/filemanager/WriteDataToFile/";
    var readDataFromFileUri = "http://localhost/SqlInjection.Prevention.API/api/filemanager/ReadDataFromFile";
    var reteiveDataUri = "http://localhost/SqlInjection.Prevention.UI/Default/DisplayDataUsingInBuiltQueryEngine/";
    var fileReadUrl = "http://localhost/SqlInjection.Prevention.UI/Default/GetFileContent/";

    var sendProcessFileRequest = function (args) {
        var url = args.url, callBack = args.callback;
        $.ajax({
            url: url,
            data: args.data,
            error: function(errorMessage) {
                return { status: "error", error: errorMessage };
            }
        }).done(function(result) {
            callBack(result);
        }).fail(function() {
            alert('errors');
            $('#loadingmessage').hide();
        });
    };

    function buildHtmlTable(selector, employees) {
        var columns = addAllColumnHeaders(employees, selector);

        for (var i = 0; i < employees.length; i++) {
            var row$ = $('<tr/>');
            for (var colIndex = 0; colIndex < columns.length; colIndex++) {
                var cellValue = employees[i][columns[colIndex]];
                if (cellValue === null) cellValue = "";
                row$.append($('<td/>').html(cellValue));
            }
            $(selector).append(row$);
        }
        sendProcessFileRequest({
            url: fileReadUrl,
            data: { 'fileSwitch': 'query' },
            callback: function (result) {
                $("#query-file-content").html(result);
                readDataFile();
            }

        });
    }


    function addAllColumnHeaders(employees, selector) {
        var columnSet = [];
        var headerTr$ = $('<tr/>');

        for (var i = 0; i < employees.length; i++) {
            var rowHash = employees[i];
            for (var key in rowHash) {
                if ($.inArray(key, columnSet) === -1) {
                    columnSet.push(key);
                    headerTr$.append($('<th/>').html(key));
                }
            }
        }
        $(selector).append(headerTr$);

        return columnSet;
    }

    $("#get-results").click(function () {
        $('#loadingmessage').show();
        $("#header ul").empty();
        $("#inbuilt-query-table").empty();
        $("#query-file-content").empty();
        $("#data-file-content").empty();

        if (!$.isNumeric($("#maxSalary").val())) {
            alert("Please enter a numeric value");
            $('#loadingmessage').hide();
            return false;
        }

        updateStatus("Query file created", true);
        sendProcessFileRequest({
            url: writingTextUri,
            data: { "maxSalary": $("#maxSalary").val() },
            callback: function (result) {
                checkFileExists(result);
            }
        });

    });

    function readDataFile()
    {
        sendProcessFileRequest({
            url: fileReadUrl,
            data: { "fileSwitch": "data" },
            callback: function (result) {
                $("#data-file-content").html(result);
            }
        });
    }

    function updateStatus(updateMessage, isSuccess) {
        var className = isSuccess ? 'message' : 'warning';
        $("#header ul").append('<li class=' + className + '><span class="tab">' + updateMessage + '</span></li>');

        if (!isSuccess) {
            $('#loadingmessage').hide();
        }
    }

    //1
    function checkFileExists(result) {
        if (result) {
            updateStatus("Query file exists", true);

            sendProcessFileRequest({
                url: checkFileExistsUri,
                data: { 'fileType': 'read' },
                callback: function (result) {
                    writeDataToFile(result);
                }

            });
        } else {
            updateStatus("Query file cannot be found", false);
        }
    }

    //2
    function writeDataToFile(result) {
        if (result) {
            updateStatus("Query file read", true);
            sendProcessFileRequest({
                url: writeDataToFileUri,
                callback: function (result) {
                    dataFileWritten(result);
                }

            });
        } else {
            updateStatus("Query file cannot be read". false);
        }
    }

    //3
    function dataFileWritten(result) {
        if (result) {
            updateStatus("Data file is written", true);
            sendProcessFileRequest({
                url: checkFileExistsUri,
                data: { 'fileType': 'write' },
                callback: function(result) {
                    readDataFromFile(result);
                }
            });
        } else {
            updateStatus("Errors occurred while writing the file", false);
        }
    }


    //4
    function readDataFromFile(result) {
        if (result) {
            updateStatus("Data file is read", true);
            sendProcessFileRequest({
                url: readDataFromFileUri,
                callback: function (result) {
                    dataInsertedToTempDb(result);
                }

            });
        } else {
            updateStatus("Data file cannot be found", false);
        }
    }

    //5
    function dataInsertedToTempDb(result) {
        if (result) {
            updateStatus("Data inserted into temp db", true);
            updateStatus("Data file is read", true);
            sendProcessFileRequest({
                url: reteiveDataUri,
                callback: function (employees) {
                    buildHtmlTable($("#inbuilt-query-table"), employees);
                }
            });
            $('#loadingmessage').hide();
        } else {
            updateStatus("Data cannot be inserted", true);
        }
    }


})(jQuery);