export function buttonLoad(btn) {
    btn.width(btn.width());
    btn.height(btn.height());
    btn.find(".text").hide();
    btn.find(".loader").show();
}

export function buttonUnload(btn) {
    btn.width(null);
    btn.height(null);
    btn.find(".text").show();
    btn.find(".loader").hide();
}
