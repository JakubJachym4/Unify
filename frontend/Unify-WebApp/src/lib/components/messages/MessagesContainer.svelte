<script lang="ts">
    import { onMount } from 'svelte';
    import type { MessageResponse } from '$lib/api/Messages/MessagesRequests';
    import MessagesList from './MessagesList.svelte';
    import NewMessage from './NewMessage.svelte';
    import { messages } from '$lib/stores/messages';
	import { get } from 'svelte/store';

    export let fixed = false;
    let messagesList: MessageResponse[] = [];
    let error = '';
    let loading = true;
    let showNewMessage = false;

    const loadMessages = async () => {
        try {
            const lastWeek = new Date(); 
            lastWeek.setDate(lastWeek.getDate() - 7);
            const date = `${lastWeek.getFullYear()}-${lastWeek.getMonth() + 1}-${lastWeek.getDate()}`;
            await messages.refresh(date);
            messagesList = get(messages).messages;
            loading = false;
        } catch (err) {
            error = (err as Error).message;
            loading = false;
        }
    };

    $: {
        $messages;  // Subscribe to store changes
        if (!loading) {
            messagesList = $messages;
            console.log(messagesList);
        }
    }

    onMount(() => {
        loadMessages();
        return () => {};
    });
</script>

<div class="messages-panel" class:fixed>
    <div class="messages-container">
        <h2>Messages</h2>
        <button 
            class="btn btn-primary mb-3 w-100" 
            on:click={() => showNewMessage = true}
        >
            New Message
        </button>
        {#if error}
            <div class="alert alert-danger" role="alert">{error}</div>
        {/if}
        {#if loading}
            <div class="d-flex justify-content-center">
                <div class="spinner-border" role="status">
                    <span class="visually-hidden">Loading...</span>
                </div>
            </div>
        {:else}
            <div class="messages-list">
                <MessagesList messages={messagesList} />
            </div>
        {/if}
    </div>
</div>

<NewMessage 
    show={showNewMessage}
    onClose={() => showNewMessage = false}
/>

<style>
    .messages-panel {
        background: white;
        border-left: 1px solid #dee2e6;
        height: 100%;
    }

    .messages-panel.fixed {
        position: fixed;
        right: 0;
        top: 56px;
        height: calc(100vh - 56px);
        padding-top: 1rem;
    }

    .messages-container {
        padding: 1rem;
        height: 100%;
        display: flex;
        flex-direction: column;
    }

    .messages-list {
        overflow-y: auto;
        flex: 1;
        padding-right: 0.5rem;
    }
</style>