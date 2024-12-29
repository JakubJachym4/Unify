<script lang="ts">
    import { onMount } from 'svelte';
    import type { MessageResponse } from '$lib/api/Messages/MessagesRequests';
    import MessageDetails from './MessageDetails.svelte';
    import { messages } from '$lib/stores/messages';
    import { get } from 'svelte/store';

    export let initialMessage: MessageResponse;
    export let show = false;
    export let onClose: () => void;
    let showDetails = true;

    let threadMessages: MessageResponse[] = [];

    const buildMessageThread = () => {
        let currentMessage = initialMessage;
        threadMessages = [currentMessage];
        const allMessages = get(messages).messages;

        // Traverse up through responding messages
        while (currentMessage.respondingToId) {
            const parentMessage = allMessages.find(m => m.messageId === currentMessage.respondingToId);
            if (parentMessage) {
                threadMessages = [parentMessage, ...threadMessages];
                currentMessage = parentMessage;
            } else {
                break;
            }
        }
    };

    onMount(() => {
        if (show) {
            showDetails = true;
            buildMessageThread();
        }
    });

    $: if (show) {
        buildMessageThread();
    }
</script>

{#if show}
<div class="modal-overlay">
    <div class="thread-container">
        <div class="thread-header">
            <h5>Message Thread</h5>
            <button type="button" class="btn-close" on:click={onClose}></button>
        </div>
        <div class="thread-content">
            {#each threadMessages as message, i}
                <div class="message-wrapper">
                    <MessageDetails 
                        message={message}
                        show={showDetails}
                        onClose={() => {showDetails = false}}
                        modalLevel={i + 2}
                        hideReplyButton={i < threadMessages.length - 1}
                    />
                    {#if i < threadMessages.length - 1}
                        <div class="thread-line"></div>
                    {/if}
                </div>
            {/each}
        </div>
    </div>
</div>
{/if}

<style>
    .modal-overlay {
        position: fixed;
        top: 0;
        left: 0;
        right: 0;
        bottom: 0;
        background-color: rgba(0, 0, 0, 0.5);
        z-index: 1050;
        display: flex;
        justify-content: center;
        align-items: center;
    }

    .thread-container {
        background: white;
        border-radius: 8px;
        width: 90vw;
        max-width: 1200px;
        height: 90vh;
        display: flex;
        flex-direction: column;
    }

    .thread-header {
        padding: 1rem;
        border-bottom: 1px solid #dee2e6;
        display: flex;
        justify-content: space-between;
        align-items: center;
    }

    .thread-content {
        flex: 1;
        overflow-y: auto;
        padding: 1rem;
    }

    .message-wrapper {
        position: relative;
        margin-bottom: 2rem;
    }

    .thread-line {
        position: absolute;
        left: 50px;
        top: 100%;
        bottom: -2rem;
        width: 2px;
        background-color: #dee2e6;
    }

    .message-wrapper:last-child .thread-line {
        display: none;
    }
</style>