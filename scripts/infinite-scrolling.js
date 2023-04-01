// https://www.meziantou.net/infinite-scrolling-in-blazor.htm
// https://github.com/meziantou/Meziantou.Framework/blob/main/src/Meziantou.AspNetCore.Components/wwwroot/InfiniteScrolling.js

export function initialize(lastIndicator, instance) {
    const options = {
        root: findClosestScrollContainer(lastIndicator),
        rootMargin: '0px',
        threshold: 0,
    };
    if (isValidTableElement(lastIndicator.parentElement)) {
        lastIndicator.style.display = 'table-row';
    }
    const observer = new IntersectionObserver(async (entries) => {
        // When the lastItemIndicator element is visible => invoke the C# method `LoadMoreItems`
        for (const entry of entries) {
            if (entry.isIntersecting) {
                observer.unobserve(lastIndicator);
                await instance.invokeMethodAsync("LoadMoreItems");
            }
        }
    }, options);
    observer.observe(lastIndicator);
    // Allow to cleanup resources when the Razor component is removed from the page
    return {
        dispose: () => infiniteScollingDispose(observer),
        onNewItems: () => {
            observer.unobserve(lastIndicator);
            observer.observe(lastIndicator);
        },
    };
}

// Find the parent element with a vertical scrollbar
// This container should be use as the root for the IntersectionObserver
function findClosestScrollContainer(element) {
    while (element) {
        const style = getComputedStyle(element);
        if (style.overflowY !== 'visible') {
            return element;
        }
        element = element.parentElement;
    }
    return null;
}

// Cleanup resources
function infiniteScollingDispose(observer) {
    observer.disconnect();
}

function isValidTableElement(element) {
    if (element === null) {
        return false;
    }
    return ((element instanceof HTMLTableElement && element.style.display === '') || element.style.display === 'table')
        || ((element instanceof HTMLTableSectionElement && element.style.display === '') || element.style.display === 'table-row-group');
}