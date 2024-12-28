import type { Handle } from '@sveltejs/kit';
import { redirect } from '@sveltejs/kit';

export const redirectNotLogged: Handle = async ({ event, resolve }) => {
    const userCookie = event.cookies.get('user');
    // If the user is not logged in and is not on the login page, redirect to login
    if (userCookie === 'false' && event.url.pathname !== '/login') {
        throw redirect(302, '/login');
    }

    return resolve(event);
};
