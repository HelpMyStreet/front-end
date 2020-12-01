
export function buttonLoad(btn) { 
    btn.find(".text").first().hide();
    btn.find(".loader").first().show();
}

export function buttonUnload(btn) {
    btn.find(".text").first().show();
    btn.find(".loader").first().hide();
}


