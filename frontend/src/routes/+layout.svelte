<script>
    import '../app.css';
    let { children } = $props();
</script>

<div class="app-container">
    {@render children()}
</div>

<style>
    .app-container {
        max-width: 1200px;
        margin: 0 auto;
        padding: var(--spacing-8) var(--spacing-4);
    }
</style>
