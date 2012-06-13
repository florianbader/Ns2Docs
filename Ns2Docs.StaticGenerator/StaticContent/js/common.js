function hideWhenNonChildClicked(element) {
	var element = $(element);
	var eventName = 'click.' + createGuid();
	setTimeout(function(){
		$('body').bind(eventName, function(event) {
			if (!$(event.target).closest(element).length) {
				element.hide();
				$('body').unbind(eventName);
			};
		});
	}, 1);
}

// http://stackoverflow.com/questions/105034/how-to-create-a-guid-uuid-in-javascript/105074#105074
function createGuid() {
    var S4 = function() {
       return (((1+Math.random())*0x10000)|0).toString(16).substring(1);
    };
    return (S4()+S4()+"-"+S4()+"-"+S4()+"-"+S4()+"-"+S4()+S4()+S4());
}



$(document).ready()
{
	var anchorHighlight = function() {
		var anchor = window.location.href.split('#')[1];
		var obg = $('#'+anchor).css('color')
		$('#'+anchor).css({'color': 'red'}).animate({
		color: obg,
	  }, 4000);
	}

	$(window).bind('hashchange', anchorHighlight);
	
	if (document.location.href.split('#').length > 1) {
		anchorHighlight();
	}
	
	
	$('.tree .tree-node').each(function() {
		var node = $(this);
		var childNode = $('.child-node:first', node);
		var expander = $('<div class="expander"></div>');
		
		expander.click(function() {
			var obj = $(this);
			
			if (!childNode.hasClass("node-closed"))
			{
				childNode.addClass('node-closed');
				expander.addClass('expander-closed');
			} else {
				childNode.removeClass('node-closed');
				expander.removeClass('expander-closed');
			}
			
			//obj.toggleClass('expander-open');
			//obj.toggleClass('expander-closed');
			//node.toggleClass('closed-node');
		});
		
		if (childNode.hasClass("node-closed")) {
			expander.addClass('expander-closed');
		}
		
		$(this).prepend(expander);
		
	});
	
	var highlightLua = function(obj)
	{
		var obj = $(obj);
		var v = obj.text()
			
		var currLineNumber = 1
		
		
		var s = v.split("\n")
		var i=0
		
		
		
		var highlighted = $("<pre />")
		highlightText(v, function(line){ 
			currLineNumber++;
			highlighted.append(line);
			highlighted.append("\n");
		}, function(){
			obj.empty();
			obj.replaceWith(highlighted);
		});	
	}
	
	$(".lua-source-code pre").each(function() {
			highlightLua(this);
		}
	);
	
	$('.lua-snippet pre').each(function() {
		highlightLua(this);
	});
	/*
	$('.bubbleInfo').each(function () {
		// options
		var distance = 10;
		var time = 250;
		var hideDelay = 500;

		var hideDelayTimer = null;

		// tracker
		var beingShown = false;
		var shown = false;
		
		var trigger = $('.trigger', this);
		var popup = $('.popup', this).css('opacity', 0);

		// set the mouseover and mouseout on both element
		$([trigger.get(0), popup.get(0)]).mouseover(function () {
		  // stops the hide event if we move from the trigger to the popup element
		  if (hideDelayTimer) clearTimeout(hideDelayTimer);

		  // don't trigger the animation again if we're being shown, or already visible
		  if (beingShown || shown) {
			return;
		  } else {
			beingShown = true;
			
			// reset position of popup box
			popup.css({
			  top: -popup.height(),
			  left: -33,
			  display: 'block' // brings the popup back in to view
			})

			// (we're using chaining on the popup) now animate it's opacity and position
			.animate({
			  top: '-=' + distance + 'px',
			  opacity: 1
			}, time, 'swing', function() {
			  // once the animation is complete, set the tracker variables
			  beingShown = false;
			  shown = true;
			});
			
			$('.lua-snippet pre', popup).each(function(){ 
				highlightLua(this);
			});
		  }
		}).mouseout(function () {
		  // reset the timer if we get fired again - avoids double animations
		  if (hideDelayTimer) clearTimeout(hideDelayTimer);
		  
		  // store the timer so that it can be cleared in the mouseover if required
		  hideDelayTimer = setTimeout(function () {
			hideDelayTimer = null;
			popup.animate({
			  top: '-=' + distance + 'px',
			  opacity: 0
			}, time, 'swing', function () {
			  // once the animate is complete, set the tracker variables
			  shown = false;
			  // hide the popup entirely after the effect (opacity alone doesn't do the job)
			  popup.css('display', 'none');
			});
		  }, hideDelay);
		});
	  });
	*/
}