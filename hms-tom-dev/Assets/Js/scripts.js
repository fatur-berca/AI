+(function($) {
	'use strict';

	if (typeof window.$ === 'undefined' || !window.$)
		throw "This theme need jQuery";

 
	/**
		//////////////////////////////// * Doc ready //////////////////////////////
	*/
	

	$(function() {
		// select 2 init
		$('.select2').select2();

		// sidebar toggle
		$('.sidebar-toggle').on('click', function(){
			$('.sidebar').toggleClass('closed');
			$('.main').toggleClass('fulled');
			$('header .logo-deck').toggleClass('closed');
			$('header .opt-head').toggleClass('closed');
			$(this).toggleClass('turn');
		});

		// close map detail toggle
		$('.close-map-detail').on('click', function(){
			$('.map-detail').removeClass('opened');
		});
		
		// toggle news n highlight
		$('.toggle-news').on('click', function(){
			$('.news-highlight').addClass('in');
		});

		$('.news-highlight-close').on('click', function(){
			$('.news-highlight').removeClass('in');
		});

		//dataTable init
		var table = $('#dtFilter').DataTable({
			"dom": 'lf<"table-overflow"t>ip',
			orderCellsTop: true
		});

		$('.dataTables_filter input').attr("placeholder", "Search Here");
	     // Setup - add a text input to each footer cell
	    // Setup - add a text input to each header cell
	    $('#dtFilter thead.filters th').each( function (index) {
	    	var title = $('#dtFilter thead.filters th').eq( $(this).index() ).text();
    		if($(this).hasClass('no-search')){
    		}
    		else if(title == 'Filter'){
    		}
    		else{
		        $(this).html( '<input type="search" class="form-control mini" placeholder="Search '+title+'" />' );
    		}
	    } ); 
	 
	    // Apply the search
	    table.columns().every(function (index) {
	        $('#dtFilter thead.filters th:eq(' + index + ') input').on('keyup change', function () {
	            table.column($(this).parent().index() + ':visible')
	                .search(this.value)
	                .draw();
	        });
	    });

	    //dataTable init
		var table2 = $('#dtFilter2').DataTable({
			"dom": 'lf<"table-overflow"t>ip',
			orderCellsTop: true
		});

		$('.dataTables_filter input').attr("placeholder", "Search Here");
	    $('#dtFilter2 thead.filters th').each( function (index) {
	    	var title = $('#dtFilter2 thead.filters th').eq( $(this).index() ).text();
    		if($(this).hasClass('no-search')){
    		}
    		else if(title == 'Filter'){
    		}
    		else{
		        $(this).html( '<input type="search" class="form-control mini" placeholder="Search '+title+'" />' );
    		}
	    } ); 

	    // Apply the search
	    table2.columns().every(function (index) {
	        $('#dtFilter2 thead.filters th:eq(' + index + ') input').on('keyup change', function () {
	            table2.column($(this).parent().index() + ':visible')
	                .search(this.value)
	                .draw();
	        });
	    });

	    //dataTable init
		var table3 = $('#dtFilter3').DataTable({
			"dom": 'lf<"table-overflow"t>ip',
			orderCellsTop: true
		});

		$('.dataTables_filter input').attr("placeholder", "Search Here");
	    $('#dtFilter3 thead.filters th').each( function (index) {
	    	var title = $('#dtFilter3 thead.filters th').eq( $(this).index() ).text();
    		if($(this).hasClass('no-search')){
    		}
    		else if(title == 'Filter'){
    		}
    		else{
		        $(this).html( '<input type="search" class="form-control mini" placeholder="Search '+title+'" />' );
    		}
	    } ); 

	    // Apply the search
	    table3.columns().every(function (index) {
	        $('#dtFilter3 thead.filters th:eq(' + index + ') input').on('keyup change', function () {
	            table3.column($(this).parent().index() + ':visible')
	                .search(this.value)
	                .draw();
	        });
	    });

		//date time picker
		$('.datepicker').datetimepicker({
			format: 'DD/MM/YYYY'
		});
		$(".yearpicker").datetimepicker({
		    format: "YYYY"
            //,startView: 'decade'
            //, minView: 'decade'
            //, viewSelect: 'decade'
            //,autoclose: true
		});

		$('.altdatepicker').datetimepicker({
		    format: 'DD-MMM-YYYY',
	   		maxDate: 'now'
		});

		$('.datetimepicker').datetimepicker();

		$('.collapse:not(#verification,#initiation,#process,#invoicing,#dispose)').on('show.bs.collapse', function () {
		    $('.collapse.in').each(function(){
		          $(this).collapse('hide');
		    });
		});
		//matchheight init
		$('.sameHeight').matchHeight();

	    //guideline window

		$('.toggle-guideline').on('click', function(){
		    event.preventDefault();
		    window.open($(this).attr("href"), "_blank");
    		//window.open($(this).attr("href"), "popupWindow", "width=400,height=400,scrollbars=yes");
		})

		//checkall dataTable
		$('.dataTable').on('change', '#checkall', function(){
			$(this).parents('.dataTable').find('tbody input:checkbox').prop('checked', $(this).prop('checked'));
		});

		$('.dataTable').on('change', '#checkall2', function(){
			$(this).parents('.dataTable').find('tbody input:checkbox').prop('checked', $(this).prop('checked'));
		});

		//collapse eachother
		$('.collapse').on('show.bs.collapse', function () {
		    $('.collapse.in').each(function(){
		        $(this).collapse('hide');
		    });
		});

		$(".filestyle").jfilestyle({
			buttonBefore: true,
			buttonText: "Browse"
		});

		$(".not-active a").on("click", function(event){
			event.preventDefault();
			event.stopPropagation();
			return false;
		})

	});

	window.onresize = function(event) {
		
	};


	//modal fullscreen
	$(".modal-fullscreen").on('show.bs.modal', function () {
	  setTimeout( function() {
	    $(".modal-backdrop").addClass("modal-backdrop-fullscreen");
	  }, 0);
	});
	$(".modal-fullscreen").on('hidden.bs.modal', function () {
	  $(".modal-backdrop").addClass("modal-backdrop-fullscreen");
	});

	
})(jQuery);
