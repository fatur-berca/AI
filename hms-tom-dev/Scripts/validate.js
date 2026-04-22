function Validate(mandatoryFields){
	var i;
	var message = "";
	for (i = 0; i < mandatoryFields.length; i++) {
	    var field = $(mandatoryFields[i])
	    if (field.val() === "" || field.val() === null || field.val() === undefined) {
			message += field.prev().html() + " must be filled \n"
		}
	}
	return message;
}