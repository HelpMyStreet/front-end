
export function buttonLoad(btn) {        
    btn.width(btn.width());
    btn.height(btn.height());
    btn.css('padding', "0");
    //btn.attr('disabled', "true");
    btn.find(".text").first().hide();
    btn.find(".loader").first().show();
    
}

export function buttonUnload(btn) {
    btn.width(null);
    btn.height(null);
    btn.css('padding', "13px 40px");
    //btn.attr('disabled', "false")
    btn.find(".text").first().show();
    btn.find(".loader").first().hide();
}


