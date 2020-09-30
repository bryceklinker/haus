import superagent from 'superagent';

async function load() {
    const response = await superagent.get('settings.json');
    return response.body;
}

export const SettingsLoader = {load};
