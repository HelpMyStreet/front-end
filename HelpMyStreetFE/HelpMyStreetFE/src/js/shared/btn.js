
export function buttonLoad(btn) {    
    btn.width(btn.width());
    btn.height(btn.height());
    btn.find(".text").first().hide();
    btn.find(".loader").first().show();
    
}

export function buttonUnload(btn) {
    btn.width(null);
    btn.height(null);
    btn.find(".text").first().show();
    btn.find(".loader").first().hide();
}


