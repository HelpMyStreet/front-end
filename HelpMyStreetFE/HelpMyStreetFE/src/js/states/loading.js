/**
 * Show loading spinner within specified container element
 * @param {string} container 
 */
export function showLoadingSpinner(container) {
  $('.loading-spinner', container).removeClass('hidden');
}

/**
 * Hide loading spinner within specified container element
 * @param {string} container 
 */
export function hideLoadingSpinner(container) {
  $('.loading-spinner', container).addClass('hidden');
}
