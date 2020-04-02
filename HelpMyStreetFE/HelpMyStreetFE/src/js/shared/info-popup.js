function initInfoPopups() {  
  const popups = document.querySelectorAll('.info-popup');
  popups.forEach(popup => {
    popup.addEventListener('click', toggleState);
  });
}

function toggleState(e) {
  const body = e.currentTarget.querySelector('.info-popup__body');
  body.classList.toggle('open');
}

initInfoPopups();