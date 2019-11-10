function remote(url, dataValue, type) {
    var result;
    $.ajax({
        url: url,
        dataType: 'json',
        type: type,
        async: false,
        contentType: 'application/json',
        data: JSON.stringify(dataValue),
        success: function (data, textStatus, jQxhr) {
            result= data;
        },
        error: function (jqXhr, textStatus, errorThrown) {
            console.log(errorThrown);
            result= null;
        }
    });
    return result;
}