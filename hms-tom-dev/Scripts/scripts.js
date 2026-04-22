$(document).ready(function() {

});	
$(window).on('load', function () {
	$('body').fadeIn('slow');
});

$(function() {
	$('a.notification-link').click(function() {
		$('.notificationContainer').fadeToggle(200);
		$('.red-notif').fadeOut();
		return false;
	});
	$(document).click(function() {
		$('.notificationContainer').fadeOut(200);
	});
	$('.notificationContainer').click(function() {
		// return false;
	});
	$('.notification-list').click(function() {
		$(this).removeClass('unread');
	});
});

		