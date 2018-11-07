$('input[autofill="off"]').disableAutofill();
$(function () {
    $('#dtpScheduledTime').datetimepicker({
        format: 'MM/DD/YYYY HH:mm',
        icons: {
            time: "fa fa-clock-o",
            date: "fa fa-calendar"
        }
    });
});

//$(function () {
//    $('#dtpAgencyContacted').datetimepicker({ maxDate: moment(), icons: { time: "fa fa-clock-o", date: "fa fa-calendar" }, format: 'MM/DD/YYYY HH:mm' }).on('dp.change', function (e) {
//        $('#dtpAgencyResponded').data("DateTimePicker").minDate(e.date)
//    });
//});
$(function () {
    $('#dtpAgencyResponded').datetimepicker({ maxDate: moment(), icons: { time: "fa fa-clock-o", date: "fa fa-calendar" }, format: 'MM/DD/YYYY HH:mm' }).on('dp.change', function (e) {
        $('#dtpAgencyArrived').data("DateTimePicker").minDate(e.date)
    });
});
$(function () {
    $('#dtpAgencyArrived').datetimepicker({ maxDate: moment(), icons: { time: "fa fa-clock-o", date: "fa fa-calendar" }, format: 'MM/DD/YYYY HH:mm' }).on('dp.change', function (e) {
        var responded = moment(new Date($("#dtpAgencyResponded").val()));
        $('#dtpAgencyArrived').data("DateTimePicker").minDate(responded)
    });
});


function UpdateAgencies() {
    //If the checkbox is checked

    if ($('#toggle-ems').is(':checked')) {
        //1.Make the transport mode as Ground EMS if it is not already
        var modeSelected = $('#ddlModes').val();
        if (modeSelected !== 'Ground EMS') {
            $('#ddlModes option:selected').remove();
            $('#ddlModes').val('Ground EMS');
        }


        //2.Empty the agencies and append the default (choose agency)
        $('#ddlAgencies').empty().append('<option selected="selected">-Choose Agency-</option>');

        //3. Call the ddlTModeChange method
        //ddlTModesChange();
    }
    else {
        //Since the checkbox is unchecked
        console.log("Chech box is unchecked...");
        var selectedTrans = $('#ddlModes').val();
        console.log("mode sent to ddlModesChange():" + selectedTrans);
        //ddlTModesChange();
    }
    //3. Call the ddlTModeChange method
    ddlTModesChange();
}

function ddlTModesChange() {
    //by default the islocal is TRUE
    var localOnly = true;
    //Get the mode of transport
    var selectedTrans = $('#ddlModes').val();
    //Check the toggle for on
    if ($('#toggle-ems').is(':checked')) {
        //the toggle is on
        console.log("toggle-ems is checked")
        //set islocal = false
        localOnly = false;
        if (selectedTrans != 'Ground EMS') {
            //$('#toggle-ems').prop('checked', false);
            console.log("setting islocal to true")
            $('#toggle-ems').prop('checked', false).bootstrapToggle('destroy').bootstrapToggle();
            localOnly = true;
        }
    }
    $.ajax({
        url: 'GetEmsAgenciesByTransMode',
        method: 'post',
        data: { transMode: selectedTrans, localUse: localOnly },
        dataType: 'json',
        success: function (response) {
            //$('#ddlAgencies').empty();
            $('#ddlAgencies').empty().append('<option selected="selected">-Choose Agency-</option>');
            $(response).each(function (index, item) {
                $('#ddlAgencies').append($('<option/>', {
                    value: item.Value,
                    text: item.Text,

                }))
            })
        }
    })
}

function ddlAgencyChange() {

    var agency = $('#ddlAgencies').val();
    if (agency === "Vanderbilt EMS") {
        $('#helpContainer').hide();
        $('#VanderbiltUnit').show();
    }
    else {
        $('#VanderbiltUnit').hide();
        $('#helpContainer').show();
    }
};

function StatusChange() {
    var status = $('#StatusDropdown').val();
    if (status === "Pending") {
        //$('#dtpAgencyContacted').val('');
        $('#dtpAgencyResponded').val('');
        $('#dtpAgencyArrived').val('');
        $('#txtError').empty();
        $('#txtError').append("Please inform agency about the change.");
        $('#txtError').show();
    };
};

//function SetAgencyContactedTime() {
//    var agency = $('#ddlAgencies').val();
//    if (agency === null || agency === "-Choose Agency-") {
//        $('#txtError').empty();
//        $('#txtError').append("Please choose an agency.");
//        $('#txtError').show();
//    }
//    else {
//        $('#txtError').empty();
//        $('#txtError').hide();

//        var s = moment().format('MM/DD/YYYY HH:mm');
//        console.log(s);
//        dtpAgencyContacted.value = s;
//    }
//}

function SetAgencyRespondedTime() {
    var agency = $('#ddlAgencies').val();
    //var agencyContacted = $('#dtpAgencyContacted').val().format('MM/DD/YYYY HH:mm');
    var maxdate = moment().format('MM/DD/YYYY HH:mm');

    if (agency === null || agency === "-Choose Agency-") {
        $('#txtError').empty();
        $('#txtError').append("Please choose an agency.");
        $('#txtError').show();
        $('#dtpAgencyResponded').datetimepicker('hide');
    }
    //if (agencyContacted === null || agencyContacted === "") {
    //    //$('#lblError').empty();
    //    $('#txtError').append("<br /> Agency should be contacted first.");
    //    $('#txtError').show();
    //    $('#dtpAgencyResponded').datetimepicker('hide');
    //}

    else {
        dtpAgencyResponded.value = maxdate;
    }

    console.log($('#dtpAgencyResponded').val());

}

function SetAgencyArrivedTime() {
    var agency = $('#ddlAgencies').val();
    var agencyResponded = $('#dtpAgencyResponded').val().format('MM/DD/YYYY HH:mm');
    //var agencyContacted = $('#dtpAgencyContacted').val().format('MM/DD/YYYY HH:mm');
    var maxdate = moment().format('MM/DD/YYYY HH:mm');
    if (agency === null || agency === "-Choose Agency-") {
        $('#txtError').empty();
        $('#txtError').append("Please choose an agency.");
        $('#txtError').show();
        $('#dtpAgencyArrived').datetimepicker('hide');
    }
    //if (agencyContacted === null || agencyContacted === "") {
    //    //$('#lblError').empty();
    //    $('#txtError').append("<br /> Agency should be contacted first.");
    //    $('#txtError').show();
    //    $('#dtpAgencyArrived').datetimepicker('hide');
    //}
    if (agencyResponded === null || agencyResponded === "") {
        //$('#lblError').empty();
        $('#txtError').append("<br /> Agency should respond first.");
        $('#txtError').show();
        $('#dtpAgencyArrived').datetimepicker('hide');
    }
    else {
        $('#txtError').empty();
        $('#txtError').append("Agency arrival page will be sent out.");
        $('#txtError').show();
        dtpAgencyArrived.value = maxdate;
        PreviewArrival();
    }

    console.log($('#dtpAgencyArrived').val());

}

function ShowAltModal(id, time, status, type) {
    //$('#dtpAgencyContacted').val("");
    $('#dtpAgencyResponded').val("");
    $('#dtpAgencyArrived').val("");
    $('#txtError').empty();
    $('#txtError').hide();
    $('#txtInfo').empty();
    $('#txtInfo').hide();
    $('#ddlAgencies').empty();
    $('#ddlModes').empty();
    $('#txtpageCaller').text("");
    $('#rowPreviewMessage').hide();
    $('#btnPreviewMessage').prop('disabled', false);
    $('#VanderbiltUnit').hide();
    $('#helpContainer').show();

    if (type === "Discharge" || "ALT Funding") {
        $('#btnPreviewMessage').show();
        $('#btnSendPage').show();
        $('#btnSendArrival').show();
    }
    if (type === "VHAN") {
        $('#btnPreviewMessage').hide();
        $('#btnSendPage').hide();
        $('#btnSendArrival').hide();
    }

    $.ajax({
        //url: '@Url.Action("GetModeAndAgency")',
        url: 'GetModeAndAgency',
        method: 'post',
        data: { DischargeRequestId: id },
        dataType: 'json',
        success: function (response) {
            $(response.TransportModes).each(function (index, item) {
                $('#ddlModes').append($('<option/>',
                    {
                        value: item.Value,
                        text: item.Text
                    }));
            });
            ddlModes.value = response.Mode;
            console.log(response.Mode);
            $(response.Agencies).each(function (index, item) {
                $('#ddlAgencies').append($('<option/>',
                    {
                        value: item.Value,
                        text: item.Text
                    }));
            });
            ddlAgencies.value = response.Agency;
            
            if (response.Agency === "Vanderbilt EMS") {
                $('#helpContainer').hide();
                $('#VanderbiltUnit').show();
                if (response.EmsUnitId != null) {
                    $('#EmsUnitId').val(response.EmsUnitId);
                }
            }
            else {
                $('#VanderbiltUnit').hide();
                $('#helpContainer').show();
            }

            //dtpAgencyContacted.value = response.AgencyContacted;
            dtpAgencyResponded.value = response.AgencyResponded;
            dtpAgencyArrived.value = response.AgencyArrived;
            txtSpecialInstructions.value = response.SpecialInstructions;
            txtNotes.value = response.Notes;
//            txtInfo.value
            $('#txtInfo').empty();
            $('#txtInfo').append(response.MessageDetails);
            $('#txtInfo').show();
            if (!response.IsLocal) {
                console.log("Local is false");
                $('#toggle-ems').prop('checked', true).bootstrapToggle('destroy').bootstrapToggle();
            }
        }
    })

    $('#editAltModal').appendTo("body").modal('show');
    dtpScheduledTime.value = time;
    StatusDropdown.value = status;
    dischargeRequestIdHidden.value = id;
    //updateContent();
};

function Print() {
    var agencyContacted = $('#dtpAgencyResponded').val();

        var dischargeRequestId = $('#dischargeRequestIdHidden').val();
        //window.location = '@Url.Action("GeneratePrintableReport")' + '?DischargeRequestId=' + dischargeRequestId;
        window.location = 'GeneratePrintableReport' + '?DischargeRequestId=' + dischargeRequestId;
        $('#editAltModal').modal('hide');


};

function printPDF() {
    var agencyContacted = $('#dtpAgencyResponded').val();

    var dischargeRequestId = $('#dischargeRequestIdHidden').val();
    var scheduledTime = $('#dtpScheduledTime').val();
    var status = $('#StatusDropdown').val()
    var mode = $('#ddlModes').val();
    var agency = $('#ddlAgencies').val();
    //var agencyContacted = $('#dtpAgencyContacted').val();
    var agencyResponded = $('#dtpAgencyResponded').val();
    var agencyArrived = $('#dtpAgencyArrived').val();
    var action = "Print";
    var actionParms = "Print";
    var specialInstructions = $('#txtSpecialInstructions').val();
    var notes = $('#txtNotes').val();
    var unit = $('#EmsUnitId').val();
    $.ajax({
        //url: '@Url.Action("UpdateAndDoNext")',
        url: 'UpdateAndDoNext',
        type: 'post',

        data: { dischargeRequestId: dischargeRequestId, scheduledTime: scheduledTime, status: status, Mode: mode, Agency: agency, AgencyResponded: agencyResponded, AgencyArrived: agencyArrived, Action: action, ActionParms: actionParms, SpecialInstructions: specialInstructions, Notes: notes, Unit:unit },
        dataType: 'json',
        success: function (response) {
            //window.location = '@Url.Action("GeneratePdf")' + '?DischargeRequestId=' + dischargeRequestId;
            //window.location = 'GeneratePdf' + '?DischargeRequestId=' + dischargeRequestId;
            window.open('GeneratePdf' + '?DischargeRequestId=' + dischargeRequestId, "newPage");

            $('#editAltModal').modal('hide');
        }
    })

}

function requestHistory() {
    var dischargeRequestId = $('#dischargeRequestIdHidden').val();
    $.ajax({
        url: 'GetRequestHistory',
        method: 'post',
        data: { RequestId: dischargeRequestId },
        dataType: 'json',
        success: function (response) {
            var table = $("#tblRequestHistory tbody");
            $.each(response, function (idx, elem) {
                //console.log(elem.CreatedOn.replace(/"\/Date\((\d+)\)\/"/g, 'new Date($1)'));
                //console.log(new Date(parseInt(elem.CreatedOn.replace(/\/+Date\(([\d+-]+)\)\/+/, '$1'))));
                //console.log(new Date(''));
                //var crd = new Date(parseInt(elem.CreatedOn.replace(/(^.*\()|([+-].*$)/g, '')));
                //var crdui = crd.getMonth() + 1 + "/" + crd.getDate() + "/" + crd.getFullYear();
                //table.append("<tr><td>" + elem.CreatedBy + "</td><td>" + crdui + "</td>   <td>" + elem.Notes + "</td></tr>");
                table.append("<tr><td>" + elem.CreatedBy + "</td><td>" + elem.CreatedDateTime + "</td>   <td>" + elem.Notes + "</td></tr>");
            });
            $('#historyModal').appendTo("body").modal('show');
        }
    });

    
};

function ClearTable() {
    $("#tblRequestHistory tbody > tr").remove();
};

function reportProblem() {
    var dischargeRequestId = $('#dischargeRequestIdHidden').val();
    $.ajax({
        url: 'GetComplaintsForUser',
        method: 'post',
        data: { RequestId: dischargeRequestId },
        dataType: 'json',
        success: function (response) {
            if (response != null) {
                console.log("response is not null")
                $('#txtComplaint').val(response.Complaint);
                console.log(response.ComplaintType);
                console.log(response.AocNotified);
                if (response.ComplaintType !== 0) {
                    complaintType.value = response.ComplaintType;
                }
                if (response.AocNotified) {
                    console.log('yes it is true')
                    $('#toggle-AocNotification').prop('checked', true).bootstrapToggle('destroy').bootstrapToggle();
                }
                else {
                    $('#toggle-AocNotification').prop('checked', false).bootstrapToggle('destroy').bootstrapToggle();
                }
            }
        }
    })
    //$('#rowComplaint').show();
    $('#problemModal').appendTo("body").modal('show');
}

function messageClipboard() {
    var dischargeRequestId = $('#dischargeRequestIdHidden').val();
    $.ajax({
        url: 'FreeTextMessage',
        method: 'post',
        data: { DischargeRequestId: dischargeRequestId },
        dataType: 'json',
        success: function (response) {
            console.log(response);
            $('#txtFreeMessage').empty();
            $('#txtFreeMessage').append(response);
            $('#txtFreeMessage').show();
        }
    });
    $('#FreeTextModal').appendTo("body").modal('show');
};

function SendFreeTextPage() {
    var dischargeRequestId = $('#dischargeRequestIdHidden').val();
    var message = $('#txtFreeMessage').val();
    var receipens = $('#txtFreeMessageRecipients').val();
    var messageType = 'FreeText';
    $.ajax({
        url: 'SendPage',
        method: 'post',
        data: { DischargeRequestId: dischargeRequestId, Message: message, Receipents: receipens, MessageType: messageType},
        dataType: 'json',
        success: function (response) {
            $('#FreeTextModal').modal('hide');
        }
    });
    //SendPage(int DischargeRequestId, string Message, string Receipents, string MessageType)
};


function SaveOrUpdateComplaint() {
    var dischargeRequestId = $('#dischargeRequestIdHidden').val();
    var complaint = $('#txtComplaint').val();
    var complaintType = $('#complaintType').val();
    var delayReason = $('#DelayReason').val();
    var isAocNotified = false;
    if ($('#toggle-AocNotification').is(':checked')) {
        isAocNotified = true;
    }
    if (delayReason === "-Choose One-") {
        delayReason = null;
    }
    $.ajax({
        url: 'InsertOrUpdateComplaint',
        method: 'post',
        data: { RequestId: dischargeRequestId, Complaint: complaint, ComplaintType: complaintType, DelayReason: delayReason, IsAocNotified: isAocNotified},
        dataType: 'json',
        success: function (response) {
            $('#problemModal').modal('hide');
        }
    })
};

function AddComplaint() {
    var requestid = $('#RequestId').val();
    var complaint = $('#txtComplaint').val();
    var complaintType = $('#complaintType').val();
    var delayReason = $('#DelayReason').val();
    var isAocNotified = false;
    if ($('#toggle-AocNotification').is(':checked')) {
        isAocNotified = true;
    }
    if (delayReason === "-Choose One-") {
        delayReason = null;
    }
    $.ajax({
        url: '../ExistingRequests/InsertOrUpdateComplaint',
        method: 'post',
        data: { RequestId: requestid, Complaint: complaint, ComplaintType: complaintType, DelayReason: delayReason, IsAocNotified: isAocNotified },
        dataType: 'json',
        success: function (response) {
            window.location.reload();
        }
    })
};

function UpdateAltStatus() {
    $('#editAltModal').modal('hide');
    var dischargeRequestId = $('#dischargeRequestIdHidden').val();
    var scheduledTime = $('#dtpScheduledTime').val();
    var status = $('#StatusDropdown').val()
    var mode = $('#ddlModes').val();
    var agency = $('#ddlAgencies').val();
    //var agencyContacted = $('#dtpAgencyContacted').val();
    var agencyResponded = $('#dtpAgencyResponded').val();
    var agencyArrived = $('#dtpAgencyArrived').val();
    var specialInstructions = $('#txtSpecialInstructions').val();
    var unit = $('#EmsUnitId').val();
    var notes = $('#txtNotes').val();
    $.ajax({
        //url: '@Url.Action("UpdateTimeAndStatus")',
        url: 'UpdateTimeAndStatus',
        method: 'post',
        data: { dischargeRequestId: dischargeRequestId, scheduledTime: scheduledTime, status: status, Mode: mode, Agency: agency, AgencyResponded: agencyResponded, AgencyArrived: agencyArrived, SpecialInstructions: specialInstructions, Notes: notes, Unit:unit },
        dataType: 'json',
        success: function (response) {
            //TODO: Call the sendpage method here
            //if (!($('#dtpAgencyArrived').val() === null || $('#dtpAgencyArrived').val() === "")) {
            //    console.log("calling the send page");
            //    SendPage();
            //}
            $('#editAltModal').modal('hide');
            window.location.reload();
        }
    })
};

function AddNewRole() {
    var roleName = $('#txtRoleName').val();
    var description = $('#txtDescription').val();
    if (roleName!="") {
        $.ajax({
            url: 'AddNewRole',
            method: 'post',
            data: { RoleName: roleName },
            dataType: 'json',
            success: function (response) {
                if (response.indexOf("Success") <= 0) {
                    $('#lblRoleError').empty();
                    $('#lblRoleError').append(response);
                    $('#lblRoleError').show();
                }
                else {
                    $('#lblRoleError').empty();
                    $('#lblRoleError').hide();
                    window.location.reload();
                }
            }
        });
    }
    else {
        $('#lblRoleError').empty();
        $('#lblRoleError').append("Please enter a valid role name");
        $('#lblRoleError').show();
    }
};
function ClearRoleError() {
    $('#lblRoleError').empty();
    $('#lblRoleError').hide();
};

function AddNewUser() {
    var firstName = $('#txtFirstName').val();
    var lastName = $('#txtLastName').val();
    var vunetid = $('#txtVunetId').val();
    var roleName = $('#RoleName').val();
    if (roleName === null || roleName === "Choose Role") {
        $('#lblAddUserError').empty();
        $('#lblAddUserError').append("Please choose a valid role");
        $('#lblAddUserError').show();
    }
    else {
        $.ajax({
            url: 'AddNewUser',
            method: 'post',
            data: { FirstName: firstName, LastName: lastName, VunetId: vunetid, RoleName: roleName },
            dataType: 'json',
            success: function (response) {
                if (response.indexOf("Success") <= 0) {
                    $('#lblAddUserError').empty();
                    $('#lblAddUserError').append(response);
                    $('#lblAddUserError').show();
                }
                else {
                    $('#lblAddUserError').empty();
                    $('#lblAddUserError').hide();
                    window.location.reload();
                }
            }
        });
    }
};

function ClearUserError() {
    $('#lblAddUserError').empty();
    $('#lblAddUserError').hide();
};
//function Close() {
//    $('#editAltModal').modal('hide');
//}

//function Close(modelId) {
//    $(modalId).modal('hide');
//}

function toggler(divId) {
    $("#" + divId).toggle();
}

//$(".collapse").collapse()

//function SaveUpdate() {
//    //$('#editAltModal').modal('hide');
//    var dischargeRequestId = $('#dischargeRequestIdHidden').val();
//    var scheduledTime = $('#dtpScheduledTime').val();
//    var status = $('#StatusDropdown').val()
//    var mode = $('#ddlModes').val();
//    var agency = $('#ddlAgencies').val();
//    var agencyContacted = $('#dtpAgencyContacted').val();
//    var agencyResponded = $('#dtpAgencyResponded').val();
//    var agencyArrived = $('#dtpAgencyArrived').val();
//    $.ajaxSetup({ async: false });
//    $.ajax({
//        //url: '@Url.Action("UpdateTimeAndStatus")',
//        url: 'UpdateTimeAndStatus',
//        type: 'post',
//        async: false,
//        data: { dischargeRequestId: dischargeRequestId, scheduledTime: scheduledTime, status: status, Mode: mode, Agency: agency, AgencyContacted: agencyContacted, AgencyResponded: agencyResponded, AgencyArrived: agencyArrived }
//        //dataType: 'json',
//        //success: function (response) {
//        //TODO: Call the sendpage method here
//        //if (!($('#dtpAgencyArrived').val() === null || $('#dtpAgencyArrived').val() === "")) {
//        //    console.log("calling the send page");
//        //    SendPage();
//        //}
//        //$('#editAltModal').modal('hide');
//        //window.location.reload();
//        //}
//    })
//    $.ajaxSetup({ async: true });
//};

//function PreviewMessage() {
//    SaveUpdate();
//    PreviewMessageAction();
//};
function PreviewArrivalMessage() {
    var dischargeRequestId = $('#dischargeRequestIdHidden').val();
    var agency = $('#ddlAgencies').val();
    //var agencyContacted = $('#dtpAgencyContacted').val().format('MM/DD/YYYY HH:mm');
    var agencyResponded = $('#dtpAgencyResponded').val().format('MM/DD/YYYY HH:mm');
    var agencyArrived = $('#dtpAgencyArrived').val().format('MM/DD/YYYY HH:mm');

    $('#txtError').empty();
    $('#pMessageType').text("");
    if (agency === null || agency === "-Choose Agency-") {
        $('#txtError').empty();
        $('#txtError').append("Please choose an agency.");
        $('#txtError').show();
    }
    //else if (agencyContacted === null || agencyContacted === "") {
    //    $('#txtError').append("<br /> Agency should be contacted.");
    //    $('#txtError').show();
    //}
    else if (agencyResponded === null || agencyResponded === "") {
        $('#txtError').append("<br /> Agency should respond.");
        $('#txtError').show();
    }
    else if (agencyArrived === null || agencyArrived === "") {
        $('#txtError').append("<br /> Agency arrival should be set.");
        $('#txtError').show();
    }
    else {
        $('#pMessageType').text("AgencyArrival");
        var msgt = $('#pMessageType').text();
        $.ajax({
            //url: '@Url.Action("PreviewMessage")',
            url: 'PreviewMessage',
            method: 'post',
            data: { DischargeRequestId: dischargeRequestId, MessageType: "Arrival", Agency: agency },
            dataType: 'json',
            success: function (response) {
                $('#rowPreviewMessage').show();
                $('#txtpageCaller').show();
                $('#pReceipents').hide();
                //$('#txtpageCaller').text(response[0]);
                txtpageCaller.value = response[0];
                $('#pReceipents').text(response[1]);
                //$('#btnSendPage').hide();
                console.log($('#pReceipents').text());
                console.log($('#txtpageCaller').text());
            }
        })
    }
}

function PreviewMessage() {
    var dischargeRequestId = $('#dischargeRequestIdHidden').val();
    var agency = $('#ddlAgencies').val();
    
    $('#txtError').empty();
    $('#pMessageType').text("");
    if (agency === null || agency === "-Choose Agency-") {
        $('#txtError').empty();
        $('#txtError').append("Please choose an agency.");
        $('#txtError').show();
    }
    else {
        $('#txtpageCaller').text("");
        $('#pReceipents').text("");
        $('#pMessageType').text("Confirmation");
        var msgt = $('#pMessageType').text();
        console.log(msgt);
        var dischargeRequestId = $('#dischargeRequestIdHidden').val();
        var scheduledTime = $('#dtpScheduledTime').val();
        var status = $('#StatusDropdown').val()
        var mode = $('#ddlModes').val();
        var agency = $('#ddlAgencies').val();
        //var agencyContacted = $('#dtpAgencyContacted').val();
        var agencyResponded = $('#dtpAgencyResponded').val();
        var agencyArrived = $('#dtpAgencyArrived').val();
        var action = "Preview";
        var actionParms = "Confirmation";
        var specialInstructions = $('#txtSpecialInstructions').val();
        var notes = $('#txtNotes').val();
        var unit = $('#EmsUnitId').val();

        $.ajax({
            //url: '@Url.Action("UpdateAndDoNext")',
            url: 'UpdateAndDoNext',
            type: 'post',

            data: { dischargeRequestId: dischargeRequestId, scheduledTime: scheduledTime, status: status, Mode: mode, Agency: agency, AgencyResponded: agencyResponded, AgencyArrived: agencyArrived, Action: action, ActionParms: actionParms, SpecialInstructions: specialInstructions, Notes: notes, Unit:unit },
            dataType: 'json',
            success: function (response) {

                $('#rowPreviewMessage').show();
                $('#txtpageCaller').show();
                $('#pReceipents').hide();
                $('#txtpageCaller').text(response[0]);
                txtpageCaller.value = response[0];
                $('#pReceipents').text(response[1]);
                console.log($('#pReceipents').text());
                console.log($('#txtpageCaller').text());

            }
        })
    }
}

function PreviewCallerMessage() {
    var dischargeRequestId = $('#dischargeRequestIdHidden').val();
    $('#txtError').empty();

    $('#pMessageType').text("");
    console.log($('#txtpageCaller').val());
    $('#txtpageCaller').val("");
    console.log($('#txtpageCaller').val());

    $('#txtpageCaller').text("");
    $('#pReceipents').text("");
    $('#pMessageType').text("MessageToCaller");
    var msgt = $('#pMessageType').text();
    console.log(msgt);
    $.ajax({
        url: 'GetCallerContact',
        method: 'post',
        data: { DischargeRequestId: dischargeRequestId },
        dataType: 'json',
        success: function (response) {
            $('#rowPreviewMessage').show();
            //$('#txtpageCaller').hide();
            $('#txtpageCaller').show();
            $('#pReceipents').hide();
            $('#pReceipents').text(response);
            console.log($('#pReceipents').text());
        }
    })
};

function PreviewArrival() {
    var dischargeRequestId = $('#dischargeRequestIdHidden').val();
    var agency = $('#ddlAgencies').val();
    //var agencyContacted = $('#dtpAgencyContacted').val().format('MM/DD/YYYY HH:mm');
    var agencyResponded = $('#dtpAgencyResponded').val().format('MM/DD/YYYY HH:mm');
    console.log($('#txtpageCaller').val());
    $('#txtpageCaller').val("");
    console.log($('#txtpageCaller').val());
    $('#pMessageType').text("");
    if (agency === null || agency === "-Choose Agency-") {
        $('#txtError').empty();
        $('#txtError').append("Please choose an agency.");
        $('#txtError').show();
    }
    //if (agencyContacted === null || agencyContacted === "") {
    //    $('#txtError').append("<br /> Agency should be contacted.");
    //    $('#txtError').show();
    //}
    if (agencyResponded === null || agencyResponded === "") {
        $('#txtError').append("<br /> Agency should respond.");
        $('#txtError').show();
    }
    else {
        $('#txtpageCaller').text("");

        $('#pReceipents').text("");
        //$('#txtpageCaller').val("");
        $('#pMessageType').text("AgencyArrival");
        $('#txtError').empty();
        $('#txtError').append("This action will close out the request.");
        $('#txtError').show();
        var msgt = $('#pMessageType').text();
        console.log(msgt);
        $.ajax({
            //url: '@Url.Action("PreviewMessage")',
            url: 'PreviewMessage',
            method: 'post',
            data: { DischargeRequestId: dischargeRequestId, MessageType: "Arrival", Agency: agency },
            dataType: 'json',
            success: function (response) {
                $('#rowPreviewMessage').show();
                $('#txtpageCaller').show();
                $('#pReceipents').hide();
                //$('#txtpageCaller').text(response[0]);
                txtpageCaller.value = response[0];
                $('#pReceipents').text(response[1]);
                //$('#btnSendPage').hide();
                console.log($('#pReceipents').text());
                console.log($('#txtpageCaller').text());
            }
        })
    }
};

function SendPage() {
    var dischargeRequestId = $('#dischargeRequestIdHidden').val();
    var messagePreview = $('#txtpageCaller').text();
    console.log(messagePreview);
    var callerMessage = $('#txtpageCaller').val();
    console.log(callerMessage);
    var receipents = $('#pReceipents').text();
    console.log(receipents);
    var messageType = $('#pMessageType').text();
    var messageToController = "";
    if (callerMessage === null || callerMessage === "") {
        messageToController = message;
    }
    else {
        messageToController = callerMessage;
    }
    console.log(messageToController);
    console.log(receipents);
    $.ajax({
        //url: '@Url.Action("SendPage")',
        url: 'SendPage',
        method: 'post',
        data: { DischargeRequestId: dischargeRequestId, Message: messageToController, Receipents: receipents, MessageType: messageType },
        dataType: 'json',
        success: function (response) {
            console.log(response);
            $('#rowPreviewMessage').hide();
            $('#txtpageCaller').text("");
            $('#pReceipents').text("");
            $('#txtpageCaller').text("");
            $('#pMessageType').text("");
        }
    })
}

function LogPage() {
    var dischargeRequestId = $('#dischargeRequestIdHidden').val();
    var messagePreview = $('#txtpageCaller').text();
    console.log(messagePreview);
    var callerMessage = $('#txtpageCaller').val();
    console.log(callerMessage);
    var receipents = $('#pReceipents').text();
    console.log(receipents);
    var messageType = $('#pMessageType').text();
    var messageToController = "";
    var pagerResponse = "Page not sent, log only";
    if (callerMessage === null || callerMessage === "") {
        messageToController = message;
    }
    else {
        messageToController = callerMessage;
    }
    console.log(messageToController);
    console.log(receipents);
    $.ajax({
        //url: '@Url.Action("SendPage")',
        url: 'LogThePage',
        method: 'post',
        data: { dischargeRequestId: dischargeRequestId, message: messageToController, receipents: receipents, messageType: messageType, pagerResponseID: pagerResponse},
        dataType: 'json',
        success: function (response) {
            console.log(response);
            $('#rowPreviewMessage').hide();
            $('#txtpageCaller').text("");
            $('#pReceipents').text("");
            $('#txtpageCaller').text("");
            $('#pMessageType').text("");
        }
    })
}

$(document).ready(function () {
    

    function updateStatus(statusVal, item) {
        item.find("td.status").text(statusVal);
    };

    function git(limit, callback) {
        var git = setInterval(function () {
            var myData = limit.find("td.timer").text();
            var status = limit.find("td.statusColor").text().trim();
            if (myData != "") {
                var countDownDate = new Date(myData);
                var one_day = 24 * 60 * 60 * 1000;              // total milliseconds in one day
                var today = new Date();
                var today_time = today.getTime();         // time in miliiseconds
                var discharge_time = countDownDate.getTime();
                
                //console.log("scheduledDate: " + myData)
                var time_diff = 0;
                var totalMins_diff = 0;
                //IF the scheduled date is in the past get the difference from today
                if (today_time > discharge_time) {
                    //console.log("expired");
                    time_diff = Math.abs(today_time - discharge_time);

                    time_diff = time_diff * -1;
                    totalMins_diff = Math.floor(time_diff / (60 * 1000));
                    //console.log("expired total minutes: " + totalMins_diff);
                }
                else {
                    //IF the scheduled date is in the future get the difference from scheduled date
                    //console.log("future");
                    time_diff = Math.abs(discharge_time - today_time);

                    totalMins_diff = Math.floor(time_diff / (60 * 1000));
                    //console.log("future total minutes: " + totalMins_diff);
                }
                SetStatusColor(limit, time_diff, status);
                callback(totalMins_diff, limit);
            }
            SetStatusColor(limit, time_diff, status);
        }, 1000);
    }

    $('#tblOne').find("tr.rowItem").each(function () {
        var v = $(this).find("td.timer").text()
        // Update the count down every 1 second
        var x = git($(this), updateStatus);
    });

    $('#tblTwo').find("tr.rowItem").each(function () {
        var v = $(this).find("td.timer").text()

        // Update the count down every 1 second
        var x = git($(this), updateStatus);
    });

    $(function () {
        $('#toggle-two').bootstrapToggle({
            on: 'Active',
            off: 'Inactive'
        });
    })

    
    
});

function SetStatusColor(limit, time_diff, status) {
    var change_me = limit.attr('class');
    //console.log(change_me);
    if (status == "Pending") {
        limit.addClass('bgPending');
    }
    if (status == "Confirmed") {
        if (time_diff < -5) {
            limit.removeClass('bgConfirmed').addClass('bgDelayed');
        }
        else if (time_diff > 43200000) {
            limit.removeClass('bgConfirmed').addClass('bgFuture');
        }
        else {
            limit.addClass('bgConfirmed');
        }
    }
    if (status == "Quote") {
        limit.addClass('bgQuote');
        limit.find(".whiteLink").removeClass();
    }
    if (status == "Scheduled") {
        if (time_diff > 43200000) {
            limit.removeClass('bgScheduled').addClass('bgFuture');
        }
        else {
            limit.addClass('bgScheduled');
        }
    }
};

$(function () {
    $('#toggle-ems').change(function () {
        UpdateAgencies();
    })
})


$(function () {
    $("#SearchString").autocomplete({
        source: 'GetFilteredCallers',
        minLength: 3
    });
});

$(function () {
    $('#RequestSearch').autocomplete({
        source: '../ExistingRequests/GetFilteredDischarges',
        minLength: 3
    });
});

$(function () {
    $('#SearchUser').autocomplete({
        source: 'GetFilteredUsers',
        minLength:3
    })
});

function AddAgencies() {
    ////selected value
    //var selectedvalue = $('#selectAllAgencies').val();
    ////selected text
    //var selectedtext = [];

    //$('#selectAllAgencies option:selected').each(function (i, selected) {
    //    selectedtext[i] = $(selected).text();
    //});

    $('#selectAllAgencies option:selected').each(function (i, selected) {
        $('#selectedAgencies').append('<option value = "' + $(selected).val() + '">' + $(selected).text() + '</option>');
    });

    $('#selectAllAgencies option:selected').remove();

    //Sorting the agencies
    var selectAllList = $('#selectedAgencies option');

    selectAllList.sort(function (a, b) {
        if (a.text.toUpperCase() > b.text.toUpperCase()) {
            return 1;
        }
        else if (a.text.toUpperCase() < b.text.toUpperCase()) {
            return -1;
        }
        else {
            return 0
        }
    });
    $('#selectedAgencies').empty().append(selectAllList);

    var selectedInsurance = $('#ddlInsurances').val();
    var values = $.map($('#selectedAgencies option'), function (e) { return e.value; });
    var agencies = values.join(',');
    //console.log(values);
    //ajax call and pass them
    $.ajax({
        url: 'AddInsAgencies',
        method: 'post',
        data: { InsuranceId: selectedInsurance, Agencies: agencies },
        dataType: 'json',
        success: function (response) {
            //window.location.reload();
        }
    });

};

function RemoveAgencies() {

    $('#selectedAgencies option:selected').each(function (i, selected) {
        $('#selectAllAgencies').append('<option value = "' + $(selected).val() + '">' + $(selected).text() + '</option>');
    });
    var selectedInsurance = $('#ddlInsurances').val();
    var agencies = [];
    $("#selectedAgencies option:selected").each(function () {
        agencies.push($(this).val());
    });
    console.log(agencies);
    $('#selectedAgencies option:selected').remove();
    //Sorting the agencies
    var selectAllList = $('#selectAllAgencies option');

    selectAllList.sort(function (a, b) {
        if (a.text.toUpperCase() > b.text.toUpperCase()) {
            return 1;
        }
        else if (a.text.toUpperCase() < b.text.toUpperCase()) {
            return -1;
        }
        else {
            return 0
        }
    });
    $('#selectAllAgencies').empty().append(selectAllList);


    $.ajax({
        url: 'RemoveInsAgencies',
        method: 'post',
        data: { InsuranceId: selectedInsurance, Agencies: agencies },
        dataType: 'json',
        success: function (response) {
            //window.location.reload();
        }
    });


    //var agencies = values.join(',');
    //console.log(agencies);
};

function ddlInsurancesChange() {
    var selectedInsurance = $('#ddlInsurances').val();
    console.log(selectedInsurance);

    $.ajax({
        url: 'GetAgenciesByInsurance',
        method: 'post',
        data: { InsProvider: selectedInsurance },
        dataType: 'json',
        success: function (response) {
            $('#selectAllAgencies').empty();
            $('#selectedAgencies').empty();

            $.each(response.all, function (i, all) {
                $('#selectAllAgencies').append('<option value=" ' + all.Value + ' ">' + all.Text + '</option>');
            });

            if ($(response.selectd).length > 0) {
                $.each(response.selectd, function (i, selectd) {
                    $('#selectedAgencies').append('<option value=" ' + selectd.Value + ' ">' + selectd.Text + '</option>');
                });
            }
        }
    })
};


function ValidatePhone(number) {
    var phoneNumber = number.value;//$('#CallerPhone').val();
    console.log(phoneNumber);
    //console.log(phoneNumber.length);
    if (ValidateNumber(phoneNumber)) {
        return true;
    }
    else {
        $('#lblError').empty();
        $('#lblError').append("Please enter a valid number");
        $(number).focus();
        return false;
    }

};

function ValidatePager(number) {
    $('#PagerPhone-error').empty();
    $('#MobilePhone-error').empty();
    var phoneNumber = number.value
    var mobileNumber = document.getElementById('MobilePhone').value.length;
    console.log(phoneNumber);

    //if (($.trim(phoneNumber).length != 10) && ($.trim(phoneNumber).length != 0)) {
    //    $('#PagerPhone-error').empty();
    //    $('#PagerPhone-error').append("Please enter a 10 digit number");
    //    //$('#CallerPhone').focus();
    //    console.log($.trim(phoneNumber).length);
    //    return false;
    //}

    if (mobileNumber == 10) {
        if (($.trim(phoneNumber).length != 10) && ($.trim(phoneNumber).length != 0)) {
            $('#PagerPhone-error').empty();
            $('#PagerPhone-error').append("Please enter a 10 digit number");
            //$('#CallerPhone').focus();
            console.log($.trim(phoneNumber).length);
            return false;
        }
    }
    else if (mobileNumber == 0) {
        if (($.trim(phoneNumber).length == 0)) {
            $('#PagerPhone-error').empty();
            $('#PagerPhone-error').append("Pager or Mobile is required");
            //$('#CallerPhone').focus();
            console.log($.trim(phoneNumber).length);
            return false;
        }
        else if (($.trim(phoneNumber).length != 10)) {
            $('#PagerPhone-error').empty();
            $('#PagerPhone-error').append("Please enter a 10 digit number");
            //$('#CallerPhone').focus();
            console.log($.trim(phoneNumber).length);
            return false;
        }
    }
    else if (($.trim(phoneNumber).length != 10)) {
        $('#PagerPhone-error').empty();
        $('#PagerPhone-error').append("Please enter a 10 digit number");
        //$('#CallerPhone').focus();
        console.log($.trim(phoneNumber).length);
        return false;
    }

    var filter = /^[0-9-+]+$/;
    if (filter.test(phoneNumber) || phoneNumber.length == 0) {
        return true;
    }
    else {
        return false;
    }
}
function ValidateMobile(number) {
    $('#MobilePhone-error').empty();
    $('#PagerPhone-error').empty();
    var phoneNumber = number.value
    var pagerNumber = document.getElementById('CallerPager').value.length;

    if (pagerNumber == 10) {
        if (($.trim(phoneNumber).length != 10) && ($.trim(phoneNumber).length != 0)) {
            $('#MobilePhone-error').empty();
            $('#MobilePhone-error').append("Please enter a valid 10 digit number");
            //$('#CallerPhone').focus();
            console.log($.trim(phoneNumber).length);
            return false;
        }
    }
    else if (pagerNumber == 0) {
        if (($.trim(phoneNumber).length == 0)) {
            $('#MobilePhone-error').empty();
            $('#MobilePhone-error').append("Pager or Mobile is required");
            //$('#CallerPhone').focus();
            console.log($.trim(phoneNumber).length);
            return false;
        }
        else if (($.trim(phoneNumber).length != 10)) {
            $('#MobilePhone-error').empty();
            $('#MobilePhone-error').append("Please enter a valid 10 digit number");
            //$('#CallerPhone').focus();
            console.log($.trim(phoneNumber).length);
            return false;
        }
    }
    else if (($.trim(phoneNumber).length != 10)) {
        $('#MobilePhone-error').empty();
        $('#MobilePhone-error').append("Please enter a valid 10 digit number");
        //$('#CallerPhone').focus();
        console.log($.trim(phoneNumber).length);
        return false;
    }


    var filter = /^[0-9-+]+$/;
    if (filter.test(phoneNumber) || phoneNumber.length == 0) {
        return true;
    }
    else {
        return false;
    }
}


function ValidateOffice(number) {
    $('#OfficePhone-error').empty();
    var phoneNumber = number.value;//$('#CallerPhone').val();

    if (($.trim(phoneNumber).length != 5) && ($.trim(phoneNumber).length != 10) && ($.trim(phoneNumber).length != 0)) {
        $('#OfficePhone-error').empty();
        $('#OfficePhone-error').append("Please enter a 5 or 10 digit number");

        //$('#CallerPhone').focus();
        console.log($.trim(phoneNumber).length);
        return false;
    }
    var filter = /^[0-9-+]+$/;
    if (filter.test(phoneNumber) || phoneNumber.length == 0) {
        return true;
    }
    else {
        return false;
    }
};



// Agency Area
$(function () {
    $('#txtAgencySearch').autocomplete({
        source: 'GetFilteredAgencies',
        minLength: 3
    });
});

$(function () {
    $("#txtRequestDestinationSearch").autocomplete({
        minLength: 5,
        source: function (request, response) {
            $.getJSON("GetFilteredDestinations", { term: request.term, Filter: $('input[name="RequestType"]:checked').val(), State: $('#State').val() }, response);
        },
        select: function (event, ui) {
            var desLabel = ui.item.label;
            var desValur = ui.item.value;
            var mrID = $('#MrN').val();
            if (mrID != null) {
                $.ajax({
                    url: 'GetDestinationAddress',
                    method: 'post',
                    data: { Destination: ui.item.label },
                    dataType: 'json',
                    async: true,
                    cache: false,
                    success: function (response) {
                        txtaddressln1.value = response.AddressLineOne;
                        txtCity.value = response.City;
                        txtZip.value = response.Zip;
                        State.value = response.State;
                        txtDestinationPhone.value = response.Phone;
                    }
                })
                
                $("#lblState").show();
                $("#State").show();
                $("#lblCity").show();
                $("#txtCity").show()
            }
        }
    });

});

$(function () {

    $("#txtpickupname").autocomplete({
        source: function (request, response) {
            $.getJSON("GetFilteredDestinations", { term: request.term, State: $('#PickupState').val() }, response);
        },
        minLength: 5,
        select: function (event, ui) {
            var desLabel = ui.item.label;
            var desValur = ui.item.value;

            var mrID = $('#MrN').val();
            if (mrID != null) {
                $.ajax({
                    url: 'GetDestinationAddress',
                    method: 'post',
                    data: { Destination: ui.item.label },
                    dataType: 'json',
                    success: function (response) {
                        txtpickupaddressline1.value = response.AddressLineOne;
                        txtpickupCity.value = response.City;
                        txtpickupZip.value = response.Zip;
                        PickupState.value = response.State;
                        txtpickupPhone.value = response.Phone;
                    }
                })
            }
        }
    });
});


$(function () {
    $('#datetimepicker1').datetimepicker({
        minDate: moment(), icons: { time: "fa fa-clock-o" },
        format: 'MM/DD/YYYY HH:mm',
        date: "fa fa-calendar"
    });
});

$(function () {
    $('#dtpApptTime').datetimepicker({
        minDate: moment(), icons: { time: "fa fa-clock-o" },
        format: 'MM/DD/YYYY HH:mm',
        date: "fa fa-calendar"
    });
});

$(function () {
    $('#txtTravelTime').datetimepicker({
        format: 'HH:mm',
        defaultDate: moment(new Date()).hours(0).minutes(0).seconds(0).milliseconds(0)
    });
});

function ShowDetails()
{
    document.getElementById('detailsId').style.display = "block";
    document.getElementById('timelineId').style.display = "none";
    document.getElementById('requestHistory').style.display = "none";
    document.getElementById('complaintsId').style.display = "none";
}

function ShowComplaints() {
    document.getElementById('detailsId').style.display = "none";
    document.getElementById('timelineId').style.display = "none";
    document.getElementById('requestHistory').style.display = "none";
    document.getElementById('complaintsId').style.display = "block";
};

function ShowTimeLine() {
    document.getElementById('detailsId').style.display = "none";
    document.getElementById('complaintsId').style.display = "none";
    document.getElementById('requestHistory').style.display = "none";
    document.getElementById('timelineId').style.display = "block";
}

function ShowRequestHistory() {
    document.getElementById('detailsId').style.display = "none";
    document.getElementById('complaintsId').style.display = "none";
    document.getElementById('timelineId').style.display = "none";
    document.getElementById('requestHistory').style.display = "block";
}

function RedirectToDischargeInfo() {
    var requestid = $('#RequestId').val();
    window.location = '../DischargeDetails/DischargeInfo' + '?DischargeRequestId=' + requestid;
}

function Delay()
{
    var ddIndex = document.getElementById('complaintType').value;
    if(ddIndex == 3)
    {
        document.getElementById('delayReason').style.display = "block";
    }
}

//function ComplaintComment(id)
//{
//    debugger;
//    var comment = document.getElementById('post-comment').value;
//    if(comment == null)
//    {
//        //throw error
//    }
//    else
//    {
//        $.ajax({
//            url: '../DischargeComplaintController/Comment',
//            method: 'post',
//            //data: {Comment: comment, RequestId: id},
//            success: function (response) {
//                if (!response) {
//                    alert("oh no!");
//                }
//            },
//            error: function (XMLHttpRequest, textStatus, errorThrown) {
//                alert("Status: " + textStatus); alert("Error: " + errorThrown);
//            }
//        })
//    }
//}

//function ComplaintResolve()
//{
//    debugger;
//}

function GoHome() {
    window.location = '../ExistingRequests/ShowExistingRequests';
};

$(function () {
    $("#destinationTypeSelection").change(function () {
        txtaddressln1.value = "";
        txtCity.value = "";
        txtZip.value = "";
        var value = $(this).val();
        var mrID = $('#MrN').val();
        var name = $('input[name="RequestType"]:checked').val();
        if (name == "Discharge") {

            if (!mrID) {
                alert("Please enter MR Number");
                document.getElementById('destinationTypeSelection').value = "Select One";
                var dval = $('#destinationTypeSelection').val();
                return false;
            }
        }
        if (value == "Address On File" && mrID != null) {
            $.ajax({
                url: '../Request/GetPatientAddress',
                method: 'post',
                data: { MrNumber: $("#MrN").val() },
                dataType: 'json',
                success: function (response) {
                    if (!response) {
                        alert("No address found for the given MR number");

                    }
                    else {
                        txtaddressln1.value = response.AddressLineOne;
                        txtCity.value = response.City;
                        txtZip.value = response.Zip;
                        State.value = response.State;
                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert("Status: " + textStatus); alert("Error: " + errorThrown);
                }
            })
            $("#lblState").show();
            $("#State").show();
            $("#lblCity").show();
            $("#txtCity").show();
        }
        else if (value == "Hospital/NursingHome") {
            $('#formdestinationdetails').show();
            $('#rowdestinationdetails').show();
            $("#lblDestination").show();
            $("#txtRequestDestinationSearch").show();
            $("#lblState").show();
            $("#State").show();
            $("#lblCity").show();
            $("#txtCity").show();

        }
        else if (value == "Other") {
            $("#lblDestination").hide();
            $("#txtRequestDestinationSearch").hide();
        }
        else if (value == "Select One") {
            $("#lblDestination").hide();
            $("#txtRequestDestinationSearch").hide();
            $("#lblState").show();
            $("#State").show();
            $("#lblCity").show();
            $("#txtCity").show();
        }
    }
    )
});

function getCityState() {
    var zipcode = $('#txtZip').val();
    if (zipcode != null) {
        $.ajax({
            url: 'GetCityState',
            method: 'post',
            data: { zip: $('#txtZip').val() },
            dataType: 'json',
            success: function (response) {
                txtCity.value = response.City;
                State.value = response.State;
            }

        })
        $("#lblState").show();
        $("#State").show();
        $("#lblCity").show();
        $("#txtCity").show()
    }
};

function getpickupCityState() {
    var zipcode = $('#txtpickupZip').val();
    if (zipcode != null) {
        $.ajax({
            url: 'GetCityState',
            method: 'post',
            data: { zip: $('#txtpickupZip').val() },
            dataType: 'json',
            success: function (response) {
                txtpickupCity.value = response.City;
                PickupState.value = response.State;
            }

        })
    }
};

function GetDirections() {
    var destination = txtaddressln1.value + " " + txtCity.value + " " + txtZip.value;

    if (destination.trim() != "") {
        var name = $('input[name="RequestType"]:checked').val();
        if (name != "Discharge") {
            var pickup = txtpickupaddressline1.value + ", " + txtpickupCity.value + ", " + txtpickupZip.value;

            window.open("http://maps.google.com/maps/dir/" + pickup + "/" + destination);
        }
        else {
            window.open("http://maps.google.com/maps?f=d&source=s_d&saddr=36+08.51N+086+48.10W&daddr=" + destination);
        }
    }
    else {
        $('#lblDirectionsError').empty();
        $('#lblDirectionsError').append("Please enter a valid destination");
        $('#lblDirectionsError').show();
    }
};

function validateMr() {
    var myLength = $("#MrN").val().length;
    var name = $('input[name="RequestType"]:checked').val();
    if (name == "Discharge") {
        if (myLength < 7) {
            $('#lblMRError').empty();
            $('#lblMRError').append("Please enter a valid MR Number");
            $('#lblMRError').show();
        }
        else {
            $('#lblMRError').hide();
            $('#lblMRError').empty();
            $.ajax({
                url: 'ValidateMrNumber',
                method: 'post',
                data: { mrn: $("#MrN").val() },
                dataType: 'json',
                error: function (response) {
                    $('#lblMRError').append("No Census Records found for this MR Number");
                    $('#lblMRError').show();
                },
                success: function (response) {
                    if (response.MrNumberFromEpi == "0") {
                        $('#lblMRError').empty();
                        $('#lblMRError').append("No records found for the given MR#, try <b>MRN By Name?</b> instead");
                        $('#lblMRError').show();
                    }
                    else if (response.InsuranceIdFromEpi == 0) {
                        $('#lblMRError').empty();
                        $('#lblMRError').append("NOTE: No Insurance records found for the given MR#");
                        $('#lblMRError').show();
                    }
                    else {
                        fName.value = response.PatientFName;
                        lName.value = response.PatientLName;
                        $('#searchGroup').show();
                        $('#searchRow').show();
                        $('#bntSearch').hide();
                    }
                }
            })
        }
    }
};

function ValidateWeight() {
    var weight = $('#patientWeight').val();
    var unit = $('#weightUnitSelection').val();
    if (weight >= 200 && unit == "KG") {
        $('#lblWeightError').empty();
        $('#lblWeightError').append("Is Bariatric Needed !!!");
        $('#lblWeightError').show();
    }
    else if (weight >= 300 && unit == "LB") {
        $('#lblWeightError').empty();
        $('#lblWeightError').append("Is Bariatric Needed !!!");
        $('#lblWeightError').show();
    }
    else if ((weight <= 200 && unit == "KG") || (weight <= 300 && unit == "LB")){
        $('#lblWeightError').empty();
        $('#lblWeightError').hide();
    }
};

function clearErrors() {
    $('#fName').empty();
    $('#lName').empty();
    $('#lblMRError').empty();
    $('#lblMRError').hide();
    $('#lblWeightError').empty();
    $('#lblWeightError').hide();
    $('#searchGroup').hide();
    $('#searchRow').hide();
};
function displaySearch() {
    $("#MrN").val("");
    $('#lblMRError').empty();
    $('#lblMRError').hide();
    $('#searchGroup').show();
    $('#searchRow').show();
    $('#fName').focus();
};

function retrieveMrn() {
    var firstname = $('#fName').val();
    var lastname = $('#lName').val();
    if (firstname != null && lastname != null) {
        $.ajax({
            url: 'RetrieveByName',
            method: 'post',
            data: { firstName: $('#fName').val(), lastName: $('#lName').val() },
            dataType: 'json',
            success: function (response) {
                if (response != "") {
                    MrN.value = response;
                }
                else {
                    alert("MR Number not found for the given patient name");
                    $('#fName').val("");
                    $('#lName').val("");
                }
            }
        })
    }
};

    function VhanSelected() {
        var value = document.querySelector('[name="RequestType"]:checked').value;
        $("input[type=text]").val("");
        $("input[type=number]").val("");
        $("input[type=checkbox]").removeAttr('checked');
        $("textarea").val("");
        $('select:not(.ignoreField) option:selected').removeAttr('selected');
        $('#patientName').show();
        $('#patientInsuranceGroup').show();
        $('#patientInsuranceRow').show();
        $('#destinationTypeSelection').hide();
        $('#lblddlDestination').hide();
        $('#rowdestinationtype').hide();
        $('#AOF').hide();
        $('#HorN').hide();
        $('#other').hide();
        $('#VUH').show();
        $('#VCH').show();
        $('#Psych').show();
        $('#MrN').hide();
        $('#lblMrN').hide();
        $('#btnMRNByName').hide();
        $('#formpickup').show();
        $('#rowpickup').show();
        $('#formpickup1').show();
        $('#rowpickup1').show();
        $('#formdestinationdetails').show();
        $('#rowdestinationdetails').show();
        $('#formdestinationinstructions').show();
        $('#rowdestinationinstructions').show();
        $('#MrN').removeAttr('required');
        $('#lblMRError').empty();
        $('#lblMRError').hide();
        $('#lblAppointmentTime').hide();
        $('#dtpApptTime').hide();
    }
    function DischargeSelected() {
        $("input[type=text]").val("");
        $("input[type=number]").val("");
        $("input[type=checkbox]").removeAttr('checked');
        $("textarea").val("");
        $('select:not(.ignoreField) option:selected').removeAttr('selected');
        $('#lblMRError').empty();
        $('#lblDirectionsError').empty();
        $('#patientName').hide();
        $('#rowdestinationtype').show();
        $('#lblddlDestination').show();
        $('#destinationTypeSelection').show();
        $('#AOF').show();
        $('#HorN').show();
        $('#other').show();
        $('#VUH').hide();
        $('#VCH').hide();
        $('#Psych').hide();
        $('#MrN').show();
        $('#lblMrN').show();
        $('#btnMRNByName').show();
        $('#formpickup').hide();
        $('#rowpickup').hide();
        $('#formpickup1').hide();
        $('#rowpickup1').hide();
        $('#formdestinationinstructions').hide();
        $('#rowdestinationinstructions').hide();
        $('#lblAppointmentTime').hide();
        $('#dtpApptTime').hide();
    }
    function ALTSelected() {

        $("input[type=text]").val("");
        $("input[type=number]").val("");
        $("input[type=checkbox]").removeAttr('checked');
        $("textarea").val("");
        $('select:not(.ignoreField) option:selected').removeAttr('selected');
        $('#lblMRError').empty();
        $('#lblMRError').hide();
        $('#patientName').hide();
        $('#patientInsuranceGroup').hide();
        $('#patientInsuranceRow').hide();
        $('#lblDirectionsError').empty();
        $('#formdestinationdetails').show();
        $('#rowdestinationdetails').show();
        $('#rowdestinationtype').hide();
        $('#AOF').hide();
        $('#HorN').hide();
        $('#other').hide();
        $('#VUH').show();
        $('#VCH').show();
        $('#Psych').show();
        $('#MrN').show();
        $('#lblMrN').show();
        $('#btnMRNByName').hide();
        $('#formpickup').show();
        $('#rowpickup').show();
        $('#formpickup1').show();
        $('#rowpickup1').show();
        $('#lblAppointmentTime').show();
        $('#dtpApptTime').show();

    }


function FormatPatientName(evt) {
    var keycode = evt.keyCode;
    if (keycode == 32) {
        $('#txtPatientName').val($('#txtPatientName').val() + ', ');
    }
}

function getRadioVal(form, name) {
    var val;
    // get list of radio buttons with specified name
    var radios = $('#options')

    // loop through list of radio buttons
    for (var i = 0, len = radios.length; i < len; i++) {
        if (radios[i].checked) { // radio checked?
            val = radios[i].value; // if so, hold its value in val
            break; // and break out of for loop
        }
    }
    return val; // return value of checked radio or undefined if none checked
}

//$(function () {
//    $('#toggle-two').bootstrapToggle({
//        on: 'Active',
//        off: 'Inactive'
//    });
//})

$(document).ready(function () {

    var updatedselect = $('input[name=RequestType]:checked').val();
    var post = $('#txtIsPostSuccess').val();
    console.log(updatedselect, post);
    if (!post) {
        if (!!updatedselect) {
            switch (updatedselect) {
                case 'Discharge':
                    DischargeSelected();
                    break;
                case 'ALT Funding':
                    ALTSelected();
                    break;
                case 'VHAN':
                    VhanSelected();
                    break;
            }
        }
    }
    else {
        if (post === "Success") {
            console.log("entering success")
            $('#modalMessage').empty();
            $('#modalMessage').append("The request was successful.");
            $('#SaveConfirmationModal').modal('show');
        }
        else if (post === "Success new caller") {
            console.log("entering new caller")
            $('#modalMessage').empty();
            $('#modalMessage').append("The request was successful. </br> The new caller needs to be entered in the Admin/Caller page.");
            $('#SaveConfirmationModal').modal('show');
        }
        else if (post === "failure") {
            console.log("entering failure")
            $('#modalMessage').empty();
            $('#modalMessage').append("No records found for the MR Number provided.");
            $('#SaveConfirmationModal').modal('show');
        }
    }
    $('#lblMRError').hide();
    $('#lblDirectionsError').empty();
});


$(document).ready(function () {
    $('#lblTransError').hide();
    $("#divReadOnlyFields :input").attr("disabled", true);
});
$(function () {
    $('#datetimepicker').datetimepicker({ format: 'MM/DD/YYYY HH:mm', date: "fa fa-calendar" });
});
$(function () {
    $('#dtpAgencyArrival').datetimepicker({ format: 'MM/DD/YYYY HH:mm', date: "fa fa-calendar" });
});
//function GenerateATNumber() {
//    var payingReason = $('#txtPayingReason').val();
//    $('#lblATError').empty();
//    $('#lblATError').hide();
//    if (payingReason === null || payingReason === "") {
//        $('#lblATError').append("Paying reason should be selected");
//        $('#lblATError').show();
//    }
//    else {

//        var dischargeTime = $('#dischargeTime').text();
//        var drID = @Model.submittedDischarge.DischargeRequestId;
//        var today = new Date(dischargeTime);
//        var month = today.getMonth() + 1;
//        var fiscalYear = "";
//        if (month > 7) {
//            fiscalYear = today.getFullYear() + 1;
//        }
//        else {
//            fiscalYear = today.getFullYear();
//        }
//        if (month < 10) {
//            month = '0' + month;
//        }
//        var atNumberGenerated = "AT" + fiscalYear + month + drID;
//        var cost = $('#txtCost').val();

//        txtATNumber.value = atNumberGenerated;
//        $.ajax({
//            url: '@Url.Action("SaveATNumber")',
//            method: 'post',
//            dataType: 'json',
//            data: {dischargeRequestId: @Model.submittedDischarge.DischargeRequestId, Reason: payingReason, ATNumber:atNumberGenerated, Cost:cost}
//        })
//    }
//}

//function ClearATNumber() {
//    var atNumber = $('#txtATNumber').val();
//    if (atNumber.trim()) {

//        var drID = @Model.submittedDischarge.DischargeRequestId;
//        $.ajax({
//            url: '@Url.Action("ClearATNumber")',
//            method: 'post',
//            dataType: 'json',
//            data: {dischargeRequestId: @Model.submittedDischarge.DischargeRequestId}
//        })
//        txtATNumber.value = "";
//        txtCost.value = "";
//        $('#txtPayingReason option:selected').remove();
//    }
//}
function ShowDemographPop() {
    $('#editDemographModal').modal('show');
    console.log($('#patientState').text());
    var selectedState = $('#patientState').text();

    $('#State option').map(function () {
        if ($(this).text() == selectedState) return this;
    }).attr('selected', 'selected');
};
function ShowEditPop() {
    $('#editCallerModal').modal('show');
    var name = $('#editcallerName').text().trim();//$('#callerFName').val() + " " + $('#callerLName').val();
    if (name.length > 0) {
        console.log("caller name is");
        callerSearch.value = name;
        txtcallerIdHidden.value = $('#callerId').text();
        $('#noCaller').hide();
    }
    else {
        $('#noCaller').show();
    }

};

function ShowInsurancePop(isVHAN) {
    console.log(isVHAN);
    $('#editInsuranceModal').modal('show');
    
    var auth = $('#authCode').text();
    if (auth !== null || auth != "") {
        txtAuthCode.value = $('#authCode').text();
        
    }
    else {
        txtAuthCode.value = "";
    }
    var payorName = $('#payorName').val();
    if (payorName !== null || payorName != "") {
        txtPatientInsuranceProvider.value = payorName;
    }
    txtPatientInsuranceId.value = $('#insuranceId').text();
    txtPlanName.value = $('#planName').text();
    txtPlanType.value = $('#planType').text();
    txtMbrInsuranceId.value = $('#mbrInsuranceId').text();
};

function ShowInsuranceDetails() {
    $('#insuranceDetailsModal').modal('show');
};

function ShowPickupPop() {

    $('#editPickupModal').modal('show');
    txtPickupModalLocationName.value = $('#txtPickupLocationName').text();
    txtPickpModalInstructions.value = $('#txtPickupInstructions').text();
    txtPikcupModaladdressln1.value = $('#txtPickupAddress').text();
    txtPickupModalCity.value = $('#txtPickupCity').text();
    PickupModalState.value = $('#txtPickupState').text();
    txtPickupModalZip.value = $('#txtPickupZip').text();
};

function ShowLocationPop() {
    $('#editLocationModal').modal('show');
    var pavillion = $('#txtPavillionCode').text();
    var unit = $('#txtUnit').text();
    var bed = $('#txtBed').text();
    $.ajax({
        url: '../DischargeDetails/GetLocations',
        method: 'post',
        dataType: 'json',
        success: function (response) {
            $(response).each(function (index, item) {
                $('#ddlPavillionCodes').append($('<option/>',
                    {
                        value: item.Value,
                        text: item.Text,
                        selected: item.Selected
                    }));
            }); 
        }
    })
};

function ddlPavillionChange() {
    // Enable ddlUnits, txtModalBed
    

    var selectedPav = $('#ddlPavillionCodes').val();
    
    if (selectedPav !== null || selectedPav !== "-Choose Building-") {
        
        $('#ddlUnits').prop('disabled', false);
        $('#txtModalBed').prop('disabled', false);

        //ddlPavillionCodes
        //Show units only for the selected pavillioin
        $.ajax({
            url: 'GetUnitsByPavillion',
            method: 'post',
            data: { Pavillion: selectedPav },
            dataType: 'json',
            success: function (response) {
                //$('#ddlAgencies').empty();
                $('#ddlUnits').empty().append('<option selected="selected">-Choose Unit-</option>');
                $(response).each(function (index, item) {
                    $('#ddlUnits').append($('<option/>', {
                        value: item.Value,
                        text: item.Text,
                    }))
                })
            }
        })
    }
};

function UpdateLocation(id) {
    var Pavillion = $('#ddlPavillionCodes').val()
    var Unit = $('#ddlUnits').val()
    var Bed = $('#txtModalBed').val()
    
    $.ajax({
        url: 'UpdateLocation',
        method: 'post',
        data: { DischargeRequestId: id, Pavillion: Pavillion, Unit: Unit, Bed: Bed},
        dataType: 'json',
        success: function (response) {
            window.location.reload();
        }
    });
};

function ShowTransportPop(id) {
    $('#editTransportModal').modal('show');
    var mot = $('#modeOfTransport').text();
    var ea = $('#emsAgency').text();
    $.ajax({
        url: '../ExistingRequests/GetModeAndAgency',
        method: 'post',
        data: { DischargeRequestId: id },
        dataType: 'json',
        success: function (response) {
            $(response.TransportModes).each(function (index, item) {
                $('#ddlModes').append($('<option/>',
                    {
                        value: item.Value,
                        text: item.Text
                    }));
            });
            ddlModes.value = response.Mode;
            $(response.Agencies).each(function (index, item) {
                $('#ddlAgencies').append($('<option/>',
                    {
                        value: item.Value,
                        text: item.Text
                    }));
            });
            ddlAgencies.value = response.Agency;        
            datetimepicker.value = $('#dischargeTime').text();
            dtpAgencyArrival.value = $('#agencyArrived').text();
            lifeSupportSelection.value = $('#lifeSupport').text();//response.CareLevel;
            txtSpecialInstructions.value = $('#splInstructions').text();//response.SpecialInstructions;
            txtNotes.value = $('#agentNotes').text();
            if (!response.IsLocal) {
                console.log("Local is false");
                $('#toggle-ems').prop('checked', true).bootstrapToggle('destroy').bootstrapToggle();
            }
        }

    });

};

function Directions(id) {
    $.ajax({
        url: '../DischargeDetails/GetAddresses',
        method: 'post',
        data: { DischargeRequestId: id },
        dataType: 'json',
        success: function (response) {
            var start = response[0];
            console.log(start);
            var end = response[1];
            console.log(end);
            window.open("http://maps.google.com/maps/dir/" + start + "/" + end);
        }
    });
    
};

function CalculateMedicareCharges(id) {
    $.ajax({
        url: '../DischargeDetails/CalculateMedicareCharges',
        method: 'post',
        data: { DischargeRequestId: id },
        dataType: 'json',
        success: function (response) {
            if (response.length > 1) {
                $('#rowCharges').show();
                $('#cabCharges').hide();
                $('#rowNoMiles').hide();
                txtAls.value = '$' + response[0].toFixed(2).replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1,");
                txtBls.value = '$' + response[1].toFixed(2).replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1,");
                txtCct.value = '$' + response[2].toFixed(2).replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1,");
                txtFwt.value = '$' + response[3].toFixed(2).replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1,");
            }
            else {
                $('#rowCharges').hide();
                $('#cabCharges').hide();
                $('#rowNoMiles').show();
            }
            $('#CalculateChargesModal').modal('show');
        }
    });
};

function CalculateCabCharges(id) {
    $.ajax({
        url: '../DischargeDetails/CalculateCabCharges',
        method: 'post',
        data: { DischargeRequestId: id },
        dataType: 'json',
        success: function (response) {
            console.log('response.length')
            console.log(response.length)
            if (response.length > 0) {
                $('#rowCharges').hide();
                $('#cabCharges').show();
                $('#rowNoMiles').hide();
                //txtYc.value = '$' + response[0].toFixed(2).replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1,");
                txtCc.value = '$' + response[0].toFixed(2).replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1,");
                if (response[1]) {
                    txtYc.value = '$' + response[1].toFixed(2).replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1,");
                }
                console.log(txtYc.value);
                console.log(txtCc.value);
            }
            else {
                $('#rowCharges').hide();
                $('#cabCharges').hide();
                $('#rowNoMiles').show();
            }
            $('#CalculateChargesModal').modal('show');
        }
    });
};

$(function () {
    $('#callerSearch').autocomplete({
        source: '../Request/GetFilteredCallers',
        appendTo: "#modalBody",
        minLength: 3,
        select: function (event, ui) {
            var label = ui.item.label;
            var value = ui.item.value;
            $.ajax({
                url: '../DischargeDetails/GetCallerDetails',
                method: 'post',
                data: { name: ui.item.label },
                dataType: 'json',
                success: function (response) {
                    txtcallerIdHidden.value = response.CallerId;
                    txtAssignment.value = response.Assignment;
                    txtphoneNumber.value = response.PhoneNumber;
                    txtpagerNumber.value = response.PagerNumber;
                    txtTitle.value = response.Title;
                }
            })
        }
    });
});

$(function () {
    $('#callerRequestSearch').autocomplete({
        source: '../Request/GetFilteredCallers',
        //appendTo: "#modalBody",
        minLength: 3,
        select: function (event, ui) {
            var label = ui.item.label;
            var value = ui.item.value;
            $.ajax({
                url: '../DischargeDetails/GetCallerDetails',
                method: 'post',
                data: { name: ui.item.label },
                dataType: 'json',
                success: function (response) {
                }
            })
        }
    });
});

$(function () {
    $("#destinationTypeSelection1").change(function () {
        txtaddressln1.value = "";
        txtCity.value = "";
        txtZip.value = "";
        var value = $(this).val();
        var mrID = $('#txtMrN').text();
        if (value == "Address On File" && mrID != null) {
            $.ajax({
                url: '../Request/GetPatientAddress',
                method: 'post',
                data: { MrNumber: $("#txtMrN").text() },
                dataType: 'json',
                success: function (response) {
                    txtaddressln1.value = response.AddressLineOne;
                    txtCity.value = response.City;
                    txtZip.value = response.Zip;
                    State.value = response.State;
                }
            })
            $("#lblState").show();
            $("#State").show();
            $("#lblCity").show();
            $("#txtCity").show();
            $("#destinationspace").hide();
            $("#lblhospitalName").hide();
            $("#txthospitalName").hide();
        }
        else if (value == "Hospital/NursingHome") {
            $("#destinationspace").show();
            $("#lblhospitalName").show();
            $("#txthospitalName").show();
            $("#lblState").show();
            $("#State").show();
            $("#lblCity").show();
            $("#txtCity").show();

        }
        else if (value == "Other") {
            $("#destinationspace").hide();
            $("#lblhospitalName").hide();
            $("#txthospitalName").hide();
        }
        else if (value == "") {
            $("#destinationspace").hide();
            $("#lblhospitalName").hide();
            $("#txthospitalName").hide();
            $("#lblState").show();
            $("#State").show();
            $("#lblCity").show();
            $("#txtCity").show();
        }
    })
});

$(function () {
    $("#txtDischargeDestinationSearch").autocomplete({
        source: function (request, response) {
            $.getJSON("../Request/GetFilteredDestinations", { term: request.term, State: $('#dState').val() }, response);
        },
        minLength: 5,
        select: function (event, ui) {
            var desLabel = ui.item.label;
            var desValue = ui.item.value;
            $.ajax({
                url: '../Request/GetDestinationAddress',
                method: 'post',
                data: { Destination: ui.item.label },
                dataType: 'json',
                success: function (response) {
                    txtaddressln1.value = response.AddressLineOne;
                    txtCity.value = response.City;
                    txtZip.value = response.Zip;
                    State.value = response.State;
                }
            })
            $("#lblState").show();
            $("#dState").show();
            $("#lblCity").show();
            $("#txtCity").show()
        }
    });
});

$(function () {
    $("#txtPickupModalLocationName").autocomplete({
        source: function (request, response) {
            $.getJSON("../Request/GetFilteredDestinations", { term: request.term, State: $('#PickupModalState').val() }, response);
        },
        minLength: 5,
        select: function (event, ui) {
            var pickupLabel = ui.item.label;
            var pickupLabel = ui.item.value;
            $.ajax({
                url: '../Request/GetDestinationAddress',
                method: 'post',
                data: { Destination: ui.item.label },
                dataType: 'json',
                success: function (response) {
                    txtPikcupModaladdressln1.value = response.AddressLineOne;
                    txtPickupModalCity.value = response.City;
                    txtPickupModalZip.value = response.Zip;
                    PickupModalState.value = response.State;
                }
            })
        }
    });

});

$(function () {
    $('#txtPatientInsuranceProvider').autocomplete({
        source: '../InsuranceAgency/GetFilteredInsAgencies',
        minLength: 3
    })
});

$(function () {
    $('#toggle-1').bootstrapToggle({
        on: 'Yes',
        off: 'No'
    });
});
$(function () {
    $('#toggle-2').bootstrapToggle({
        on: 'Yes',
        off: 'No'
    });
});
$(function () {
    $('#toggle-3').bootstrapToggle({
        on: '',
        off: ''
    });
});
$(function () {
    $('#toggle-4').bootstrapToggle({
        on: '',
        off: ''
    });
});

$(function () {
    $('#txtInsAgencySearch').autocomplete({
        source: 'GetFilteredInsAgencies',
        minLength: 3
    });
});


$(function () {
    $('#InsuranceDescriptionSearch').autocomplete({
        source: 'GetInsDescriptions',
        minLength: 0
    }).focus(function () {
        $(this).autocomplete("search");
    });
});

function ShowContactPop() {
    $('#addContactModal').modal('show');
};

function AddInsuranceContact() {
    $('#addContactModal').modal('hide');
    $.ajax({
        url: '../InsuranceAgency/AddInsuranceContact',
        method: 'post',
        data: { InsuranceID: $('#InsuranceId').val(), Name: $('#txtContactName').val(), Email: $('#txtContactEmail').val(), Phone: $('#txtContactPhone').val(), Fax: $('#txtContactFax').val(), Note: $('#txtContactNotes').val() },
        dataType: 'json',
        success: function (response) {
            window.location.reload();
        }
    });
};

function SaveInsuranceInfo() {
    var pr = false;
    var pw = false;
    if ($('#toggle-1').is(':checked')) {
        pr = true;
    };
    if ($('#toggle-2').is(':checked')) {
        pw = true;
    }
    console.log(pr, pw);
    $.ajax({
        url: '../InsuranceAgency/UpdateInsuranceInfo',
        method: 'post',
        data: { InsuranceID: $('#InsuranceId').val(), Name: $('#txtInsuranceName').val(), Description: $('#txtInsuranceDescription').val(), Phone: $('#txtInsurancePhone').val(), Fax: $('#txtInsuranceFax').val(), PreAuth: pr, OwnPaper: pw , Instructions: $('#txtInsuranceInstructions').val() },
        dataType: 'json',
        success: function (response) {
            window.location.reload();
        }
    });
};

function ValidateInsuranceName() {
    var insEntered = $('#txtInsAgencySearch').val();
    $.ajax({
        url: '../InsuranceAgency/ValidateInsuranceName',
        method: 'post',
        data: { Name: insEntered },
        dataType: 'json',
        success: function (response) {
            if (response) {
                $('#lblInsError').show();
                $('#lblInsError').empty();
                $('#lblInsError').append("Insurance Agency with name "+ insEntered + " already exists");
            }
            else {
                $('#lblInsError').empty();
                $('#lblInsError').hide();
            }
        }
    });
};
function AddNewInsurance() {
    var pr = false;
    var pw = false;
    if ($('#toggle-1').is(':checked')) {
        pr = true;
    };
    if ($('#toggle-2').is(':checked')) {
        pw = true;
    }
    $.ajax({
        url: '../InsuranceAgency/AddNewInsuranceAgency',
        method: 'post',
        data: { Name: $('#txtInsAgencySearch').val(), Description: $('#InsuranceDescriptionSearch').val(), Phone: $('#InsurancePhone').val(), Fax: $('#InsuranceFax').val(), PreAuth: pr, OwnPaper: pw, Instructions: $('#txtInsuranceInstructions').val()},
        dataType: 'json',
        success: function (response) {
            if (response > 0) {
                txtNewInsId.value = response;
                $('#InsAddConfirmationModal').modal('show');
                //window.location = '../InsuranceAgency/GetInsuranceAgencyDetails?insAgencyId='+ response;
            }
            else {
                $('#lblInsError').show();
                $('#lblInsError').empty();
                $('#lblInsError').append("There was an error, please check the data and re-try");
            }
        }
    });
};
function EditNewIns(id) {
    window.location = '../InsuranceAgency/GetInsuranceAgencyDetails?insAgencyId=' + id;
};

function GenerateATNumber(id) {
    var payingReason = "";//$('#ddlPayingReason').val();
    var payor = $('#ddlPayors').val();
    if (payor.indexOf("VUMC") === 0 || payor.indexOf("VCH") === 0 || payor.indexOf("VPH") === 0) {
        payingReason = $('#ddlPayingReason').val();
    }
    else {
        payingReason = $('#txtpayingReason').val();
    }
    var dischargeTime = $('#dischargeTime').text();
    $('#lblATError').empty();
    $('#lblATError').hide();
    if (payor === null || payor === "") {
        //debugger;
        $('#lblATError').append("Payor and Paying reason should be selected");
        $('#lblATError').show();
    }
    else if (dischargeTime === null || dischargeTime === "") {
        $('#lblATError').append("Discharge Time should be selected");
        $('#lblATError').show();
    }

    else {

        var drID = id;
        var today = new Date(dischargeTime);
        var month = today.getMonth() + 1;
        var fiscalYear = "";
        if (month > 7) {
            fiscalYear = today.getFullYear() + 1;
        }
        else {
            fiscalYear = today.getFullYear();
        }
        if (month < 10) {
            month = '0' + month;
        }
        var atNumberGenerated = "AT" + fiscalYear + month + drID;
        var cost = $('#txtCost').val();
        console.log(cost);
        txtATNumber.value = atNumberGenerated;
    }
};

function PayorChange() {
    var payor = $('#ddlPayors').val();
    $('#lblATError').empty();
    $('#lblATError').hide();
    if (payor.indexOf("VUMC") === 0 || payor.indexOf("VCH") === 0 || payor.indexOf("Stallworth") === 0) {
        ResetPayingReason();
        $('#txtpayingReason').val("");
        $('#tdPayingReasonTxt').hide();
        $("#tdpayingReasonddl").show();
        $('#ddlPayingReason option[value="VPH Transport"]').hide();
        $('#ddlPayingReason option[value=""]').attr("selected", true);
        $("#ddlPayingReason").prop("disabled", false);
    }
    else if (payor.indexOf("VPH") === 0) {
        ResetPayingReason();
        $('#txtpayingReason').val("");
        $('#tdPayingReasonTxt').hide();
        $("#tdpayingReasonddl").show();
        $('#ddlPayingReason option[value="VPH Transport"]').attr("selected", true);
        $('#ddlPayingReason option[value="VPH Transport"]').show();
        $("#ddlPayingReason").prop("disabled", true);
    }
    else if (payor.indexOf("Complex Care") === 0) {
        ResetPayingReason();
        $("#tdpayingReasonddl").hide();
        $('#tdPayingReasonTxt').show();
        $('#txtpayingReason').val("Special Case");
    }
    else if (payor.indexOf("Other") === 0) {
        ResetPayingReason();
        $("#tdpayingReasonddl").hide();
        $('#tdPayingReasonTxt').show();
        $('#txtpayingReason').val("");
    }
    else {
        ResetPayingReason();
        $('#tdPayingReasonTxt').hide();
        $("#ddlPayingReason").show();
        $("#ddlPayingReason").prop("disabled", true);
        $('#txtpayingReason').val("");
    }
};
function ResetPayingReason() {
    $('#ddlPayingReason >option').each(function () {
        if (this.selected) {
            console.log(this.text);
            this.removeAttribute('selected', false);
        };
    });
};
function ResetPayor() {
    $('#ddlPayor >option').each(function () {
        if (this.selected) {
            console.log(this.text);
            this.removeAttribute('selected', false);
        };
    });
};
function VPHSelected() {
    $('#ddlPayingReason >option').each(function () {
        if (this.selected) {
            console.log(this.text);
            this.removeAttribute('selected', false);
        };
    });
    $('#ddlPayingReason option[value="VPH Transport"]').attr("selected", true);
    $('#ddlPayingReason option[value="VPH Transport"]').show();
    $("#ddlPayingReason").prop("disabled", true);
};

function VUMCSelected() {
    $('#ddlPayingReason >option').each(function () {
        if (this.selected) {
            console.log(this.text);
            this.removeAttribute('selected', false);
        };
    });
    $('#ddlPayingReason option[value="VPH Transport"]').hide();
    $('#ddlPayingReason option[value=""]').attr("selected", true);
    $("#ddlPayingReason").prop("disabled", false);
};

function ShowDestinationPop() {
    $('#editDestinationModal').modal('show');
    var des = $('#destinationType').text();
    //debugger;
    document.getElementById("destinationTypeSelection1").value = des;
    //debugger;
    txtaddressln1.value = $('#destinationAddress').text();
    txtCity.value = $('#destinationCity').text();
    dState.value = $('#destinationState').text();
    txtZip.value = $('#destinationZip').text();
    txtPhone.value = $('#destinationPhone').text();
    txtDischargeDestinationSearch.value = $('#destinationName').text();
    txtMiles.value = $('#destinationMiles').text();
    txtTravelTime.value = $('#destinationTravelTime').text();
    txtdestInstructions.value = $('#destInstructions').text();
    if (des == "Hospital/NursingHome") {
        $("#destinationspace").show();
        $("#lblhospitalName").show();
        $("#txthospitalName").show();
    }
};



function ClearATNumber(id) {
    var atNumber = $('#txtATNumber').val();
    debugger;
    if (atNumber.trim()) {
        txtATNumber.value = "";
        txtCost.value = "";
        SaveNumbers(id);
        ResetPayingReason();
        ResetPayor();
        $("#ddlPayingReason").prop("disabled", true);
        $('#tdPayingReasonTxt').hide();
    }
};

function SaveNumbers(id) {
    var payingReason = "";//$('#ddlPayingReason').val();
    var payor = $('#ddlPayors').val();
    if (payor.indexOf("VUMC") === 0 || payor.indexOf("VCH") === 0 || payor.indexOf("VPH") === 0) {
        payingReason = $('#ddlPayingReason').val();
    }
    else {
        payingReason = $('#txtpayingReason').val();
    }
    var atNumberGenerated = $('#txtATNumber').val();
    var cost = $('#txtCost').val();
    var invoice = $('#txtInvoiceNumber').val();
    var PO = $('#txtPONumber').val();
    $('#lblATError').empty();
    $('#lblATError').hide();
    if (payor === null || payor === "") {
        //debugger;
        $('#lblATError').append("Payor and Paying reason should be selected");
        $('#lblATError').show();
    }
    else if (atNumberGenerated === null || atNumberGenerated === "") {
        $('#lblATError').append("Please generate the ATNumber to save");
        $('#lblATError').show();
    }
    else {

        $.ajax({
            url: '../DischargeDetails/SaveNumbers',
            method: 'post',
            dataType: 'json',
            data: { dischargeRequestId: id, Payor: payor, Reason: payingReason, ATNumber: atNumberGenerated, Cost: cost, Invoice: invoice, PO: PO },
            success: function (response) {
                window.location.reload();
            }
        })
    }
};

function ClearNumbers(id) {
    var payor = $('#ddlPayors').val();
    if (payor === null || payor === "") {

    }
    else {
        $.ajax({
            url: '../DischargeDetails/ClearNumbers',
            method: 'post',
            dataType: 'json',
            data: { DischargeRequestId: id },
            success: function (response) {
                window.location.reload();
            }
        })
    }
};

function UpdatePatient() {
    $('#editDemographModal').modal('hide');
    var fname = $('#txtpFirstName').val();
    var lname = $('#txtpLastName').val();
    var dob = $('#txtpDateOfBirth').val();
    var age = $('#txtpAge').val();
    var weight = $('#txtpWeight').val();
    var social = $('#txtpSocial').val();
    var address = $('#txtpAddress').val();
    var city = $('#txtpCity').val();
    var state = $('#State').val();
    var zip = $('#txtpZip').val();
    var phone = $('#txtpPhone').val();
    var mrn = $('#txtpMrn').val();
    var csn = $('#txtpCaseNumber').val();
    var pid = $('#txtPatientIdHidden').val();

    $.ajax({
        url: '../DischargeDetails/UpdatePatientInfo',
        method: 'post',
        data: { FirstName: fname, LastName: lname, DOB: dob, Age: age, Weight: weight, Phone: phone, Social: social, Address: address, City: city, State: state, Zip: zip, Mrn: mrn, Csn: csn, Pid: pid },
        dataType: 'json',
        success: function (response) {
            window.location.reload();
        }
    });
};

function UpdateCaller(id) {

    $('#editCallerModal').modal('hide');
    //debugger;
    $.ajax({
        url: '../DischargeDetails/UpdateCallerInfo',
        method: 'post',
        data: { callerId: $('#txtcallerIdHidden').val(), dischargeRequestId: id },
        dataType: 'json',
        success: function (response) {
            //debugger;
            $('#editCallerModal').modal('hide');
            window.location.reload();
        }
    })
};

function UpdatePickup(id) {
    $('#editPickupModal').modal('hide');
    $.ajax({
        url: '../DischargeDetails/UpdatePickupInformation',
        method: 'post',
        data: {
            PickupLocationName: $('#txtPickupModalLocationName').val(), PickupInstructions: $('#txtPickpModalInstructions').val(), PickupAddress: $('#txtPikcupModaladdressln1').val(), PickupCity: $('#txtPickupModalCity').val(), PickupState: $('#PickupModalState').val(), PickupZip: $('#txtPickupModalZip').val(), dischargeRequestId: id
        },
        dataType: 'json',
        success: function (response) {
            window.location.reload();
        }
    })
};

function UpdateTransportData(id) {

    var mo = $('#ddlModes').val();
    var ag = $('#ddlAgencies').val();
    var dt = $('#datetimepicker').val();
    var aa = $('#dtpAgencyArrival').val();
    var st = $('#ddlStatus').val();
    var si = $('#txtSpecialInstructions').val();
    var ls = $('#lifeSupportSelection').val();
    var nt = $('#txtNotes').val();
    console.log(nt);
    var cks = [];
    $('#mckSplNeeds input:checked').each(function () {
        cks.push($(this).val());
    });
    var selected = cks.join(',');
    console.log(selected);
    //debugger;
    if (ag.trim() == "") {
        $('#lblTransError').empty();
        $('#lblTransError').append("Agency should be selected before update");
        $('#lblTransError').show();
    }
    else {
        console.log(cks);
        //debugger;
        $('#editTransportModal').modal('hide');
        //debugger;
        $.ajax({
            url: '../DischargeDetails/UpdateTransportInformation',
            method: 'post',
            data: { Mode: mo, Agency: ag, DischargeTime: dt, RequestStatus: st, AgencyArrival: aa, dischargeRequestId: id, specialInstructions: si, lifeSupport: ls, specialNeeds: cks, notes: nt },
            dataType: 'json',
            success: function (response) {
                //debugger;
                window.location.reload();
            }
        })
    }
};

function UpdateInsurance(id) {
    debugger;
    var abc = $('#txtMbrInsuranceId').val();
    if (!abc) {
        abc = 0;
    };
    var pip = $('#txtPatientInsuranceId').val();
    if (!pip) {
        pip = 0;
    };
    console.log($('#txtPatientInsuranceId').val());
    console.log($('#txtPatientInsuranceId').val());
    var authCode = document.getElementById('txtAuthCode').value;
    var icdCode = document.getElementById('txtDiagnosis').value;
    //debugger;
    $('#editInsuranceModal').modal('hide')
    $.ajax({
        url: '../DischargeDetails/UpdateInsuranceInfo',
        method: 'post',
        data: { MbrInsuranceId: abc, PayorName: $('#txtPatientInsuranceProvider').val(), InsuranceId: pip, PlanName: $('#txtPlanName').val(), PlanType: $('#txtPlanType').val(), DischargeRequestId: id, AuthCode: authCode, ICDCode: icdCode },
        dataType: 'json',
        success: function (response) {
            window.location.reload();
        }
    })
};

function UpdateDestination(id) {
    $('#editDestinationModal').modal('hide');
    $.ajax({
        url: '../DischargeDetails/UpdateDestinationInfo',
        method: 'post',
        data: { Address: $('#txtaddressln1').val(), City: $('#txtCity').val(), State: $('#dState').val(), Zip: $('#txtZip').val(), DestinationName: $('#txtDischargeDestinationSearch').val(), DestinationType: $('#destinationTypeSelection1').val(), DestinationInstructions: $('#txtdestInstructions').val(), Miles: $('#txtMiles').val(), Phone: $('#txtPhone').val(), TravelTime: $('#txtTravelTime').val(), dischargeRequestId: id },
        dataType: 'json',
        success: function (response) {
            window.location.reload();
        }
    })
};

$(function () {
    $('#txtSearch').autocomplete({
        source: '../Request/GetFilteredDestinations',
        minLength: 5
    });
});

$(function () {
    $('#txtDiagnosis').autocomplete({
        source: '../DischargeDetails/GetICDCodes',
        minLength: 5
    });
});

function ChangePlaceHolder() {
    var selection = $('#SearchByOption').val();
    console.log(selection);
    if (selection === "Name") {
        $('#RequestSearch').removeAttr('placeholder');
        $('#RequestSearch').attr("placeholder", "John Doe");
    }
    else if (selection === "MRN") {
        $('#RequestSearch').removeAttr('placeholder');
        $('#RequestSearch').attr("placeholder", "12345689");
    }
    else if (selection === "AT#") {
        $('#RequestSearch').removeAttr('placeholder');
        $('#RequestSearch').attr("placeholder", "AT123546789");
    }
};
