var pos = 0;

function onMemberToggle() {
	var membersToDisplay = [];
	
	$('#member-name-filter-container input[type=checkbox]:checked').each(function() { 
		membersToDisplay.push('.' + $(this).attr('value'));
	});
	
	if (membersToDisplay.length == 0) {
		$('#member-name-filter-container input[type=checkbox]').each(function() { 
			membersToDisplay.push('.' + $(this).attr('value'));
		});
	}
	
	if ($('#member-name-filter-container :focus').length == 0)
	{
		$('#member-name-filter').focus();
	}
	
	displayMembers(membersToDisplay);
}

function displayMembers(membersToDisplay) {
	var membersToDisplayStr = membersToDisplay.join();
	/*var members = $('#members');
	
	var membersClone = members.clone();
	membersClone.find('.member').each(function() {
		if ($(this).is(membersToDisplayStr)) {
			$(this).parent().show();
		} else {
			$(this).parent().hide();
		}
	});
	
	members.replaceWith(membersClone);*/
	buildAutoComplete(membersToDisplay);
}

function displayS(parent, names)
{
	parent.empty();
	for (var n=0; n<names.length; n++) {
		var l = $('<li>' + '<a href="#' + names[n].link + '">' + names[n].name + '</a></li>');
		parent.append(l);
	}
}

function buildTasks(parent)
{
	var button = $('<button>Tasks</button>');
	button.button();
	
	parent.append(button);
}

function buildGetters(parent)
{
	var li = $('<li />');
	
	li.addClass('filter-popup-activator');
	var button = $('<a href="javascript:void(0)">Getters/Setters</a>');
	li.append(button);
	//button.button();
	button.click(function() {
		var c = $('<div />');
		c.addClass('filter-popup')
		var k = {};
		$('.member').each(function(){
			var name = $.trim($('.member-name:first', this).text());
			var id = $('.member-name:first', this).attr('id');
			var result = /:Get([A-Z]\w*)\(\s*\)/.exec(name);
			if (result) {
				var getter = result[1];
				if (!k[getter]) {
					k[getter] = {};
				}
				k[getter].getter = id;
			}
			
			result = /:Set([A-Z]\w*)\(.*\)/.exec(name);
			if (result) {
				var setter = result[1];
				if (!k[setter]) {
					k[setter] = {};
				}
				k[setter].setter = id;
			}
		});
		var ul = $('<ul />');
		var arr = [];
		for (var key in k)
		{
			arr.push(k[key]);
			k[key].name = key;
		}
		arr.sort(function(a, b) { 
			if (a.name < b.name) return -1;
			if (a.name > b.name) return 1;
			return 0;
		});
		for (var i=0; i<arr.length; i++) {
			var d = arr[i];
			var getter = d.getter;
			var setter = d.setter;
			var s = "";
			if (getter) {
				s += ' <a href="#' + getter +'">getter</a>';
			} else {
				s += ' <span class="inactive">getter</span>';
			}
			
			if (setter) {
				s += ' <a href="#' + setter + '">setter</a>';
			} else {
				s += ' <span class="inactive">setter</span>';
			}

			var li = $('<li>' + d.name + '</li>');
			li.prepend($('<span class="getter-setter-links">' + s + '</span>'));

			ul.append(li);
		}
		c.append(ul);
		ul.css({
			padding: '14px'
		});
		parent.after(c);
		c.addClass('filter-popup');
		c.attr('id', 'getters-setters');
		
		
		var dt = /*(button.offset().right - button.parent().offset().right) */-c.width() *0.5 + button.width() * 0.5;
		c.css({
			right: 0 +'px'
		});
		hideWhenNonChildClicked(c);
	});
	
	parent.append(li);
}

function buildMembersDeclaredInTable(parent)
{
	var pt = $.trim($('.page-header:first').text());
	var li = $('<li />');
	
	li.addClass('filter-popup-activator');
	
	var button = $('<a href="javascript:void(0)">Members declared in ' + pt + '</a>');
	li.append(button);
	
	parent.append(li);
	button.click(function() {
		var c = $('<div />');
		var arr = [];
		$('.member').each(function(){
			var name = $.trim($('.member-name:first', this).text());
			name = /([\.:]\w+)/.exec(name)[1];
			var id = $('.member-name:first', this).attr('id');
			var o = $(this).hasClass('originating-member');
			
			if (o) {
				arr.push({name:name, link:id});
			}
		});
		var ul = $('<ul />');
		
		for (var i=0; i<arr.length; i++) {
			var d = arr[i];
			
			var li = $('<li><a href="#' + d.link + '">' + d.name + '</a></li>');
			
			li.css({
				'margin-bottom': '5px'
			});
			ul.append(li);
		}
		ul.css({
			'list-style': 'none',
			'margin': 0,
			padding: 0
		});
		c.append(ul);
		c.addClass('filter-popup');
		c.attr('id', 'declared-in-class');
		parent.after(c);
		ul.css({overflow: 'auto', 'max-height': '400px',})
		var dp = c;
		var v = (dp.offset().left + dp.outerWidth())
		var v2 = (button.offset().left + button.outerWidth())
		var v3 = (dp.outerWidth() - button.outerWidth()) / 2;
		var a = Math.abs(v - v2 - v3)
		
		c.css({
			right: a
		});
		hideWhenNonChildClicked(c);
	});
	
	
}



var sections;
function addAlphabeticalList(parent)
{
	var li = $('<li />');
	
	li.addClass('filter-popup-activator');
	
	var button = $('<a href="javascript:void(0)">A-Z</a>');
	li.append(button);
	parent.append(li);
	button.addClass('filter-popup-activator');
	var build = function() {
		if (sections)
		{
			$('#a-z-members').show();
			hideWhenNonChildClicked('#a-z-members');
			return;
		}
		var d = $("<div />");
		var dp = $("<div />");
		
		d.append(u);
		var t = {}
		sections = t;
		var k = []
		$('.member').each(function(){
			var name = $.trim($('.member-name:first', this).text());
			name = /([\.:]\w+)/.exec(name)[1];
			var firstCharacter = /[\.:]_*(\w)/i;
			var result = firstCharacter.exec(name);
			if (result) {
				var c = result[1].toUpperCase();
				if (isNumber(c)) {
					c = '#';
				}
				if (!t[c]) {
					t[c] = [];
					k.push(c);
				}
				t[c].push({name:name, link:$('.member-name:first', this).attr('id')});
			}
		});
		
		k.sort();
		var u = $('<ul class="a-z-index"/>');
			d.append(u) 
		for (var i=0; i<k.length; i++) {
			var c = k[i];
			
		}

		var al = $('<ul class="alphabet-indices"/>');
		var alphabet = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ#';
		for (var i=0; i<alphabet.length; i++) {
			var s = alphabet.charAt(i);
			var ss = $('<li class="inactive">'+s+'</li>');
			if (t[s]) {
				ss = $('<a href="javascript:void(0);">' + s + '</a>');
				ss.click(function(){
					displayS(u, sections[$(this).text()])
					/*u.css({
						'max-height': '300px',
						'overflow': 'auto',
						'margin': 0,
						'padding': 0
					});*/
				});
				var os = ss;
				ss = $('<li />');
				ss.append(os);
			}
			//ss.css({display: 'inline', padding: '0 2px'});
			al.append(ss);
		}
		displayS(u, sections[k[0]])
		dp.append(d);
		dp.append(al);
		
		al.css({
			'text-align': 'center',
			'margin': 0,
			'padding': 0,
			'list-style': 'none'
		});
		dp.attr('id', 'a-z-members');
		dp.addClass('filter-popup');
		parent.after(dp);
		var v = (dp.offset().left + dp.outerWidth())
		var v2 = (button.offset().left + button.outerWidth())
		var v3 = (dp.outerWidth() - button.outerWidth()) / 2;
		var a = Math.abs(v - v2 - v3)
		
		dp.css({
			right: a,
		});
		
		hideWhenNonChildClicked(dp);
		//displayS(u, sections['A'])
	}
	
	button.click(build);
	
}


$(document).ready(function() {
	var ic = $('<div style="display: inline" />');
	$('.bar').append(ic)
	
	$('#member-name-filter').remove();
	
	
	var os = $('#member-name-filter').width();
	var filter = $('#member-name-filter');
	
	var isShrinking = false;
	var isGrowing = false;
	
	var z = $('<ul id="filter-popups" style="float: right"></div>');
	$('.bar').append(z);
	addAlphabeticalList(z);
	buildMembersDeclaredInTable(z);
	buildGetters(z);
	//buildTasks($('.bar'));
	
	
	var sidebarStartPosition = $('#sidebar').offset();
	//alert(filterStartPosition.Top)
	
	/*
	$(document).scroll(function() {
		
		if ($(window).scrollTop() > sidebarStartPosition.top) {
			if ($('#sidebar').css('position') != 'fixed')
			{
				
				$('#sidebar').css({top: "10px", position: 'fixed' });
				var delta = $('.ui-autocomplete').offset().top - $('#sidebar input').offset().top;
				$('.ui-autocomplete').css({top: filter.offset().top + filter.outerHeight() - $(window).scrollTop() + 'px' });
			}
		} else {
			
			$('#sidebar').css({ position: 'absolute' });
			$('.ui-autocomplete').css({top: filter.offset().top + filter.outerHeight() - $(window).scrollTop() + 'px' });
		}
	});
	*/
	$( "#member-name-filter-container .search-filter" ).buttonset({
	});
	
	
});

// http://stackoverflow.com/questions/18082/validate-numbers-in-javascript-isnumeric/1830844#1830844
function isNumber(n) {
  return !isNaN(parseFloat(n)) && isFinite(n);
}