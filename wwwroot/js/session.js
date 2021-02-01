
$(document).ready(function () {
  GetTasks();
});

const BASEURL = window.location.origin + "/sprint";

//SignalR configurations
var connection = new signalR.HubConnectionBuilder()
  .withUrl("/scrumHub")
  .configureLogging(signalR.LogLevel.Information)
  .build();

async function start() {
  try {
    await connection.start();
    console.log("SignalR Connected.");
  } catch (err) {
    console.log(err);
    setTimeout(start, 5000);
  }
};

start();

var participants = [];

//when participants received the session if participant already exist, can not created , it has to be logout this  session after that try again.
connection.on("ReceiveMessage", (key, email, isOwner, image,) => {

  var result = participants.includes(email);
  if (result == false) {

    $("#participants").append(`
                   <div id="id_email" class="my-3 p-3 bg-white rounded shadow-sm" style="float: left;">
                      <h6 class="border-bottom pb-2 mb-0" style="vertical-align:middle;margin:0px 40px">Participant</h6>
                      <div class="d-flex text-muted pt-3" style="vertical-align:middle;margin:0px 55px">
                      <button type="button" class="btn btn-primary"> <span class="badge badge-light" id="btnScore_${email}">-</span></button>
                      </div>

                      <div class="pb-3 mb-0 small lh-sm border-bottom w-100">
                          <div class="d-flex justify-content-between">
                          <strong class="text-gray-dark" style="vertical-align:middle;margin:0px 40px">${email.split('@')[0]}</strong>
                          </div>
                          <img src="${isOwner == true ? "/img/sheriff.png" : image}" class="d-block" weight="55" height="55" style="vertical-align:middle;margin:0px 50px"/>
                      </div>
                   </div>
  `);

    if (isOwner == true) {
      document.getElementById("participants").style.visibility = "hidden";
    }
    participants.push(email);
  } else {
    document.getElementById("btnScore_" + email).innerText = "";
  }
});

connection.on("ReceiveFromMemberMessage", (score, key, email) => {

  var scoreSpan = document.getElementById("btnScore_" + email);
  if (scoreSpan != null)
    scoreSpan.textContent = score;
})
var taskNumber = "";
//when owner create new task send to visible information to members
connection.on("ChangesStateFibonacciButtons", (visible, owner, task, isUpdated) => {

  if (visible == true && owner == false) {
    var numerator = document.getElementById("fibonacciNumbers");
    if (numerator != null) {
      numerator.style.visibility = "visible";
      document.getElementById("effortTaskNumber").innerText = "Vote for request number: " + task;
      taskNumber = task;
    }
  }

  if (isUpdated) {
    var numerator = document.getElementById("fibonacciNumbers");
    if (numerator != null) {
      numerator.style.visibility = "hidden";
      document.getElementById("effortTaskNumber").innerText = "";
    }
  }
});


function UpdateTaskScore(score, email) {

  var serviceURL = BASEURL + '/notifymemberscore?score=' + score + "&key=" + GetUrlParametersByName('key') + "&email=" + email + "&task=" + taskNumber;

  $.ajax({
    type: "POST",
    url: serviceURL,
    contentType: "application/json; charset=utf-8",
    dataType: "json",
    success: successFunc,
    errror: errorFunc
  });
  function successFunc(data, func) {
    console.log("update task score succeess");
  }

  function errorFunc() {
    alert('error');
  }
}

function ReConnect() {
  location.reload();
}

//GetTasks db üzerinde kayıtlı task varsa page load anında çağrılır. 
function GetTasks() {

  key = GetUrlParametersByName('key');

  var serviceURL = BASEURL + '/gettasks?key=' + key;

  $.ajax({
    type: "GET",
    url: serviceURL,
    contentType: "application/json; charset=utf-8",
    dataType: "json",
    success: successFunc,
    errror: errorFunc
  });

  function successFunc(data, status) {

    console.log(data);
    if (data == null) {
      console.log("no tasks available");
    }
    else {
      $.each(data, function (index, value) {
        for (let index = 0; index < value.tasks.length; index++) {
          FillTask(value.tasks[index].id, value.tasks[index].name, value.tasks[index].score);
        }

        CalculateSprintData(data.result.count, data.result.totalScore);
      });
    }
  }
  function errorFunc(error) {
    alert(error);
  }
}

/*
CreateTask owner yeni talep istediği gönderdiğinde çağırılır. İçerisinde dinamik oluşan table yapısı ve 
Fibonacci numalarının visible or hidden olmasını sağlayan SignalR request i vardır.
*/
function CreateTask(groupId) {
  var serviceURL = BASEURL + '/CreateTask/';

  var data = {};
  data.key = document.getElementById('btnCreateTask').getAttribute('data-itemid');
  data.task = $('#tbTaskNo').val();
  data.groupid = groupId;
  if (data.task == "") return;

  $.ajax({
    type: "POST",
    url: serviceURL,
    data: JSON.stringify(data),
    contentType: "application/json; charset=utf-8",
    dataType: "json",
    success: successFunc,
    error: errorFunc
  });

  function successFunc(data, status) {
    FillTask(data.result.task.id, data.result.task.name, data.result.task.score);
    //score buttons will be visible
    window.confirm("Are you sure ?");
    if (confirm("Confirm!")) {
      document.getElementById("tbTaskNo").innerText = "";
      ChangeButtonState(true, false, data.result.task.name);
      CalculateSprintData(data.result.count, data.result.totalScore);
    } else {
      txt = "You pressed Cancel!";
    }
  }
  function errorFunc(error) {
    alert(error);
  }
}

function Remove(id) {
  var serviceURL = BASEURL + '/taskremove?id=' + id;

  $.ajax({
    type: "POST",
    url: serviceURL,
    contentType: "application/json; charset=utf-8",
    dataType: "json",
    success: successFunc,
    error: errorFunc
  });

  function successFunc(data, func) {
    var row = document.getElementById("tr_" + id);
    row.parentNode.removeChild(row);
    CalculateSprintData(data.result.count, data.result.totalScore);
  }

  function errorFunc() {
    alert('error');
  }
}

function TaskScoreUpdate(id) {
  var serviceURL = BASEURL + '/taskscoreupdate?id=' + id + "&score=" + document.getElementById("score_" + id).value;

  $.ajax({
    type: "POST",
    url: serviceURL,
    contentType: "application/json; charset=utf-8",
    dataType: "json",
    success: successFunc,
    error: errorFunc
  });

  function successFunc(data, func) {
    toastr.success('You succeed it');
    CalculateSprintData(data.result.count, data.result.totalScore);

    var fibonacciNumbers = document.getElementById("fibonacciNumbers");

    if (fibonacciNumbers != null) {
      fibonacciNumbers.style.visibility = "hidden";
      document.getElementById("sprintinformation").innerHTML;
    }

    var score = document.getElementById("btnScore_" + GetUrlParametersByName('email'));
    if (score != null) {
      document.getElementById("btnScore_" + GetUrlParametersByName('email')).innerText = "";
    }
  }

  function errorFunc() {
    alert('error');
  }
}

/*
Page load and create new task sonrasında sprint bilgilerini günceller
*/
function CalculateSprintData(count, totalScore) {
  if (document.getElementById("sprintinformation") != null)
    document.getElementById("sprintinformation").innerHTML = `Total Task : <strong>${count} </strong> Total Score:<strong> ${totalScore} </strong>`;
}

function FillTask(id, name, score) {
  $('#myTable').append(`<tr id="tr_${id}">
                          <td>${name}</td>
                          <td><input type="number" class="form-control" id="score_${id}" placeholder="Score" value="${score}"/></td>
                          <td><input type="button" value="Remove" id="btn_${id}" class="btn btn-outline-danger" data-itemid="${id}" onClick="Remove(${id})"/></td>
                          <td><input type="button" value="Update" id="btn_${id}" class="btn btn-outline-warning" onClick="TaskScoreUpdate(${id})"/></td>
                        </tr>`);
}

/*
  Create Task methodu çağrıldığında eğer başarılı bir şekilde oluşturulduysa bu method call edilir. Member'lara buttonları aktifleştirir.
*/
function ChangeButtonState(state, owner, task) {
  var serviceURL = BASEURL + '/activatebuttons?visible=' + state + '&owner=' + owner + "&task=" + task;
  $.ajax({
    type: "POST",
    url: serviceURL,
    contentType: "application/json; charset=utf-8",
    dataType: "json",
    success: successFunc,
    error: errorFunc
  });

  function successFunc(data, func) {
    console.log(data);
  }

  function errorFunc() {
    alert('error');
  }
}

function CloseMeeting(id) {
  console.log(id);
}

//client 
function GetNumber(elem) {
  UpdateTaskScore(elem.textContent, GetUrlParametersByName('email'));
}

function GetUrlParametersByName(name) {
  const queryString = window.location.search;
  const urlParams = new URLSearchParams(queryString);
  const param = urlParams.get(name);
  return param;
}

/*
  CreateTask methodunda oluşturulan ve database aktifleştirlmesi için kullanırlı ama belki kaldırılır.
*/
function Update() {
  ChangeButtonState(false, false);
}
