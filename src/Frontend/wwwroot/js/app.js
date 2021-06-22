var PrimeViewJS = PrimeViewJS || {};

PrimeViewJS.GetScrollOffset = function () {
	return window.pageYOffset;
}

PrimeViewJS.SetScrollOffset = function (offset) {
  window.scrollTo({
    top: offset,
    left: 0,
    behavior: 'auto'
  });
}
