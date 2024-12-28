<script lang="ts">
    import { onMount } from 'svelte';
    import { getLastMessagesByDate, type MessageResponse } from '$lib/api/Messages/MessagesRequests';
    import MessagesList from '$lib/components/messages/MessagesList.svelte';

    let messages: MessageResponse[] = [];
    let error = '';
    let loading = true;

    //TODO: load from globalUsers store
    const loadMessages = async () => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            const lastWeek = new Date(); 
            lastWeek.setDate(lastWeek.getDate() - 7);
            const date = `${lastWeek.getFullYear()}-${lastWeek.getMonth() + 1}-${lastWeek.getDate()}`;
            messages = await getLastMessagesByDate(date, token);
            console.log('Fetched messages:', messages); // Add this line
            loading = false;
        } catch (err) {
            error = (err as Error).message;
            loading = false;
        }
    };

    onMount(loadMessages);
</script>

<div class="container mt-4">
    <div class="row">
        <div class="col-md-8">
            <h1>Student Dashboard</h1>
            <!-- Add other student dashboard content here -->
        </div>
        <div class="col-md-4">
            <h2>Messages from Last Week</h2>
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
                <MessagesList {messages} />
            {/if}
        </div>
    </div>
</div>

<style>
    .container {
        display: flex;
        flex-direction: row;
    }
    .col-md-8 {
        flex: 2;
    }
    .col-md-4 {
        flex: 1;
    }
</style>