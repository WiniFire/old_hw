@if($paginator->hasPages())
    <nav class="pagination" aria-label="Страницы"><a @class(['button', 'secondary', 'disabled' => $paginator->onFirstPage()]) href="{{ $paginator->previousPageUrl() ?? '#' }}">← Назад</a><span>Страница {{ $paginator->currentPage() }} из {{ $paginator->lastPage() }}</span><a @class(['button', 'secondary', 'disabled' => ! $paginator->hasMorePages()]) href="{{ $paginator->nextPageUrl() ?? '#' }}">Далее →</a></nav>
@endif
