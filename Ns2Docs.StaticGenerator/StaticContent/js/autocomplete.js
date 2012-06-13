
function buildAutoComplete(membersToDisplay) {
	
	var filter = $('#member-name-filter');
	filter.autocomplete('destroy');
	var tags = [];
	var mt = '.member';
	var membersToDisplayStr;
	if (membersToDisplay)
	{
		membersToDisplayStr = membersToDisplay.join();
	}
	$('.member').each(function(){
		
		if (membersToDisplay && !$(this).is(membersToDisplayStr)) {
			
		} else {
			var name = $('.member-name:first', this);
			var text = name.text();
			text = text.replace(/\s{2,}/g, " ");
			text = $.trim(text)
			
			var link = name.attr('id');
			
			var classes = $(this).attr('class').split(/\s+/);
			
			var library;
			for (var i=0; i<classes.length; i++) {
				var item = classes[i];
				if (item.substring(0, 'library-'.length) == 'library-') {
					library = item;
					break;
				}
			}
			
			tags.push({label: text,
				link: link,
				library: library
				}
			)
		}
	});
	
	
	$('#member-name-filter').autocomplete({
		source: tags,
		select: function(event, ui) {
			var href = window.location.href.split('#')[0];
			window.location.href = href + '#' + ui.item.link;
			$('#member-name-filter').blur();
			return false;
		},
		open: function(event, ui) {
			pos = $(this).offset()
			
		},
		position: { my : "left bottom", at: "left top" }
	}).focus(function() {
		if (!$.trim(filter.val()) == "") {
			filter.autocomplete("search")
		}
	}).data( "autocomplete" )._renderItem = function( ul, item ) {
		var content = $('<a><div class="autocomplete-' + item.library +'"></div>' + item.label + "</a>")
		
		var li = $('<li />')
		li.data( "item.autocomplete", item );
		
		li.append(content);
		
		li.appendTo(ul);
		

		
		return li
	};
	
	filter.autocomplete("search")
}

$(document).ready(function() {
	$('.bar').append('<input>');
	$('.bar').append('<div class="search-icon"></div>');
	//$('.bar').append(ic)
	$('.bar input').attr('id', 'member-name-filter');
	buildAutoComplete();
});