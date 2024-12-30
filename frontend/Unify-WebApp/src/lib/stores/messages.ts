import { writable } from 'svelte/store';
import type { MessageResponse } from '$lib/api/Messages/MessagesRequests';
import { getLastMessagesByDate, getNonExpiredNotifications } from '$lib/api/Messages/MessagesRequests';

function createMessagesStore() {
    const { subscribe, set } = writable<MessageResponse[]>([]);

    return {
        subscribe,
        set,
        refresh: async (date: string) => {
            const token = localStorage.getItem('token');
            if (token) {
                try {
                    const messages = await getLastMessagesByDate(date, token);
                    const notificationsResponse = await getNonExpiredNotifications(token);
                    const notifications = notificationsResponse.informationMessageResponses || [];
                    console.log(notifications);
                    messages.messages.push(...notifications);
                    set(messages);
                } catch (error) {
                    console.error('Failed to refresh messages:', error);
                }
            }
        }
    };
}

export const messages = createMessagesStore();