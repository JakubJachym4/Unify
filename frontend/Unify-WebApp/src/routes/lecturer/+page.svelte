<script lang="ts">
	import HomeworkAssignmentManagement from '$lib/components/lecturer/assignments/HomeworkAssignmentManagement.svelte';
	import ClassSessionManagement from '$lib/components/lecturer/ClassSessionManagement.svelte';
    import LecturerCourseManagement from '$lib/components/lecturer/LecturerCourseManagement.svelte';
	import LecturerDashboard from '$lib/components/lecturer/LecturerDashboard.svelte';
    import MyClassesManagement from '$lib/components/lecturer/MyClassesManagement.svelte';
    import MessagesContainer from '$lib/components/messages/MessagesContainer.svelte';
    
    
    let showMessages = true;
    let activeComponent = 'dashboard';

    const toggleMessages = () => {
        showMessages = !showMessages;
    };

    const setActiveComponent = (component: string) => {
        activeComponent = component;
    };

    let showingSession = false;
    let showingAssignment = false;
    let selectedClassOfferingId: string | null = null;
</script>

<div class="lecturer-container">
    <nav class="lecturer-nav bg-light border-bottom mb-4">
        <div class="container-fluid">
            <div class="d-flex justify-content-between align-items-center py-3">
                <div class="nav-buttons">
                    <button 
                        class="btn {activeComponent === 'dashboard' ? 'btn-primary' : 'btn-outline-primary'} me-2"
                        on:click={() => setActiveComponent('dashboard')}
                    >
                        Dashboard
                    </button>
                    <button 
                        class="btn {activeComponent === 'courses' ? 'btn-primary' : 'btn-outline-primary'} me-2"
                        on:click={() => setActiveComponent('courses')}
                    >
                        My Courses
                    </button>
                    <button 
                        class="btn {activeComponent === 'classes' ? 'btn-primary' : 'btn-outline-primary'} me-2"
                        on:click={() => activeComponent = 'classes'}
                    >
                        My Classes
                    </button>
                    <!-- Add more lecturer-specific buttons here -->
                </div>
                <button 
                    class="btn {showMessages ? 'btn-primary' : 'btn-outline-primary'}"
                    on:click={toggleMessages}
                >
                    {showMessages ? 'Hide Messages' : 'Show Messages'}
                </button>
            </div>
        </div>
    </nav>

    <div class="content-container">
        <div class="main-content {showMessages ? 'with-messages' : ''}">
            {#if activeComponent === 'dashboard'}
                {#if showingSession && selectedClassOfferingId}
                    <ClassSessionManagement 
                        classOfferingId={selectedClassOfferingId}
                        onBack={() => {
                            showingSession = false;
                            selectedClassOfferingId = null;
                        }}
                    />
                {:else if showingAssignment && selectedClassOfferingId}
                    <HomeworkAssignmentManagement
                        classOfferingId={selectedClassOfferingId}
                        onBack={() => {
                            showingAssignment = false;
                            selectedClassOfferingId = null;
                        }}
                    />
                {:else}
                    <LecturerDashboard
                        on:openSession={(event) => {
                            selectedClassOfferingId = event.detail.classOfferingId;
                            showingSession = true;
                        }}
                        on:openAssignment={(event) => {
                            selectedClassOfferingId = event.detail.classOfferingId;
                            showingAssignment = true;
                        }}
                    />
                {/if}
            {:else if activeComponent === 'courses'}
                <LecturerCourseManagement />
            {:else if activeComponent === 'classes'}
                <MyClassesManagement />
            {/if}
            
        </div>
        
        {#if showMessages}
            <div class="messages-sidebar">
                <MessagesContainer fixed={false} />
            </div>
        {/if}
    </div>
</div>

<style>
    .lecturer-container {
        height: calc(100vh - 56px);
        overflow: hidden;
    }

    .lecturer-nav {
        position: sticky;
        top: 0;
        z-index: 1020;
    }

    .content-container {
        display: flex;
        height: 100%;
    }

    .main-content {
        flex: 1;
        padding: 1rem;
        overflow-y: auto;
        transition: width 0.3s ease;
    }

    .main-content.with-messages {
        width: calc(100% - 400px);
    }

    .messages-sidebar {
        width: 400px;
        border-left: 1px solid #dee2e6;
        height: 100%;
        overflow-y: auto;
    }

    @media (max-width: 768px) {
        .main-content.with-messages {
            width: 100%;
        }

        .messages-sidebar {
            position: fixed;
            right: 0;
            top: 56px;
            bottom: 0;
            background: white;
            z-index: 1030;
        }
    }
</style>