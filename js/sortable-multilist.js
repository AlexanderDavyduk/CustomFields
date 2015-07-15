function SortableMultilistSelectAll(id) {
  list1 = jQuery('#' + id + '_sortable1').children();
  list2 = jQuery('#' + id + '_sortable2');
  
  jQuery(list1).appendTo(list2);
}

function SortableMultilistDeselectAll(id) {
  list1 = jQuery('#' + id + '_sortable1');
  list2 = jQuery('#' + id + '_sortable2').children();
  
  jQuery(list2).appendTo(list1);
}

function SetValue(id) {
  list2 = jQuery('#' + id + '_sortable2').children();
  var x = '';
  
  jQuery.each(list2, function(){
    var atr = jQuery(this).attr('value');
	if (typeof atr != "undefined") {
	  atr = atr.split(';');
	  var id = atr[0].split('&');
	  x = x + id[1] + '|';
	}
  });
  
  if (x.length > 0){
     x = x.slice(0,-1);
  }
  
  jQuery('#' + id + '_Value').val(x);
}

function FastSorting(id){
  var selected = jQuery('#' + id + '_SortBy').val();
  ul = jQuery('#' + id + '_sortable2')
  if (selected == 'Name'){
	jQuery('#' + id + '_sortable2 > li').sort(SortByName).appendTo(ul);
  }
  if (selected == 'DateCreated'){
    jQuery('#' + id + '_sortable2 > li').sort(SortByDateCreated).appendTo(ul);
  }
  if (selected == 'DateUpdated'){
    jQuery('#' + id + '_sortable2 > li').sort(SortByDateUpdated).appendTo(ul);
  }  
}

function SortByDateCreated(a, b) {
  var value1 = jQuery(a).attr('value').split(';');
  var date1 = value1[1].split('&');
  var date1 = date1[1].split('.');
  date1 = new Date(date1[2], date1[1] - 1, date1[0]);
  var value2 = jQuery(b).attr('value').split(';');
  var date2 = value2[1].split('&');
  date2 = date2[1].split('.');
  date2 = new Date(date2[2], date2[1] - 1, date2[0]);

  return date1 < date2 ? 1 : -1;
}

function SortByDateUpdated(a, b) {
  var value1 = jQuery(a).attr('value').split(';');
  var date1 = value1[2].split('&');
  var date1 = date1[1].split('.');
  date1 = new Date(date1[2], date1[1] - 1, date1[0]);
  var value2 = jQuery(b).attr('value').split(';');
  var date2 = value2[2].split('&');
  date2 = date2[1].split('.');
  date2 = new Date(date2[2], date2[1] - 1, date2[0]);

  return date1 < date2 ? 1 : -1;
}

function SortByName(a, b){
  return jQuery(a).text().toUpperCase().localeCompare(jQuery(b).text().toUpperCase());
}