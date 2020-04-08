
export function getAppSetting(settingKey) {
    return $.ajax({
        type: "GET",
        url: "/api/Configuration/appSetting",
        data: {
            key: settingKey,
        }
    });
}