const BASEURL = window.location.origin + "/group/";

function CloseGroup(groupId) {

    var serviceURL = BASEURL + 'close?groupId=';

    $.ajax({
      type: "POST",
      url: serviceURL + groupId,
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      success: successFunc,
      error: errorFunc
    });

    function successFunc(data, status) {
      if (data == true)
        location.reload();
      else
        alert('something is wrong');
    }
    function errorFunc() {
      alert('error');
    }
  }

  function RemoveGroup(groupId) {
    var serviceURL =  BASEURL + 'delete?groupId=';

    $.ajax({
      type: "POST",
      url: serviceURL + groupId,
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      success: successFunc,
      error: errorFunc
    });

    function successFunc(data, status) {
      if (data == true) {
        location.reload();
        document.getElementById("btnJoin").disabled = true;
      }
      else
        alert('Before close the group after that remove it');
    }
    function errorFunc() {
      alert('error');
    }
  }