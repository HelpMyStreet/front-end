
export function buttonLoad(btn) { 
    //btn.width(btn.width());
    //btn.height(btn.height());
    //btn.attr('disabled', "true");
    btn.find(".text").first().hide();
    btn.find(".loader").first().show();
    
}

export function buttonUnload(btn) {
    //btn.width(null);
    //btn.height(null);
    //btn.attr('disabled', "false")
    btn.find(".text").first().show();
    btn.find(".loader").first().hide();
}


