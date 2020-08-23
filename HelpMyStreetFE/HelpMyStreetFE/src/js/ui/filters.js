const toggleButtons = document.querySelectorAll(".btn__toggle-show");

toggleButtons.forEach((btn) => {
  btn.addEventListener("click", () => {
    const target = document.getElementById(
      btn.getAttribute("data-target-item")
    );

    if (!target.classList.contains("filter--show")) {
      target.classList.remove("applied");
      if (btn.classList.contains("toggle--exclusive")) {
        // Collapse others in group when toggle--exclusive defined
        const otherTargets = document.querySelectorAll(
          `.${btn.getAttribute("data-target-group")}`
        );

        otherTargets.forEach((t) => {
          if (t !== target) {
            t.classList.remove("filter--show");
          }
        });
      }
    }

    if (target) {
      target.classList.toggle("filter--show");
    }
  });
});

// Apply filter button
const applyButtons = document.querySelectorAll(".btn--apply-filter");
applyButtons.forEach((b) => {
  b.addEventListener("click", () => {
    // close menu
    const f = document.getElementById(b.getAttribute("data-target-filter"));
    if (f) {
      f.classList.remove("filter--show");
      f.classList.add("applied");
    }
  });
});
