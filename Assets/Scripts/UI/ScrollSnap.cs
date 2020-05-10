using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Mask))]
[RequireComponent(typeof(ScrollRect))]
public class ScrollSnap : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler {

    [Tooltip("Container where the images are located")]
    public RectTransform container;
    [Tooltip("Button to the next image")]
    public GameObject btnNext;
    [Tooltip("Button to the previous image")]
    public GameObject btnPrev;

    // Set starting page index - starting from 0
    private int startingPage = 0;
    // Threshold time for fast swipe in seconds
    private float fastSwipeThresholdTime = 0.3f;
    // Threshold time for fast swipe in (unscaled) pixels
    private int fastSwipeThresholdDistance = 100;
    // How fast will page lerp to target position
    private float decelerationRate = 10f;

    // fast swipes should be fast and short. If too long, then it is not fast swipe
    private int _fastSwipeThresholdMaxLimit;

    private ScrollRect _scrollRectComponent;
    private RectTransform _scrollRectRect;

    // number of pages in container
    private int _pageCount;
    private int _currentPage;

    // whether lerping is in progress and target lerp position
    private bool _lerp;
    private Vector2 _lerpTo;

    // target position of every page
    private List<Vector2> _pagePositions = new List<Vector2>();

    // in draggging, when dragging started and where it started
    private bool _dragging;
    private float _timeStamp;
    private Vector2 _startPosition;

    void Start() {
        _scrollRectComponent = GetComponent<ScrollRect>();
        _scrollRectRect = GetComponent<RectTransform>();
        _pageCount = container.childCount;

        _lerp = false;

        // init
        SetPagePositions();
        SetPage(startingPage);

        // prev and next buttons
        if (btnNext)
            btnNext.GetComponent<Button>().onClick.AddListener(() => { NextScreen(); });

        if (btnPrev)
            btnPrev.GetComponent<Button>().onClick.AddListener(() => { PreviousScreen(); });
	}

    void Update() {
        // if moving to target position
        if (_lerp) {
            // prevent overshooting with values greater than 1
            float decelerate = Mathf.Min(decelerationRate * Time.deltaTime, 1f);
            container.anchoredPosition = Vector2.Lerp(container.anchoredPosition, _lerpTo, decelerate);
            // time to stop lerping?
            if (Vector2.SqrMagnitude(container.anchoredPosition - _lerpTo) < 0.25f) {
                // snap to target and stop lerping
                container.anchoredPosition = _lerpTo;
                _lerp = false;
                // clear also any scrollrect move that may interfere with our lerping
                _scrollRectComponent.velocity = Vector2.zero;
            }
        }
    }

    private void SetPagePositions() {
        // screen width in pixels of scrollrect window
        int width = (int)_scrollRectRect.rect.width;
        // center position of all pages
        int offsetX = width / 2;
        // total width
        int containerWidth = width * _pageCount;
        // limit fast swipe length - beyond this length it is fast swipe no more
        _fastSwipeThresholdMaxLimit = width;

        // set width of container
        int containerHeight = 0;
        Vector2 newSize = new Vector2(containerWidth, containerHeight);
        container.sizeDelta = newSize;
        Vector2 newPosition = new Vector2(containerWidth / 2, containerHeight / 2);
        container.anchoredPosition = newPosition;

        // delete any previous settings
        _pagePositions.Clear();

        // iterate through all container childern and set their positions
        for (int i = 0; i < _pageCount; i++) {
            RectTransform child = container.GetChild(i).GetComponent<RectTransform>();
            Vector2 childPosition;
            
            childPosition = new Vector2(i * width - containerWidth / 2 + offsetX, 0f);
            
            child.anchoredPosition = childPosition;
            _pagePositions.Add(-childPosition);
        }
    }

    private void SetPage(int aPageIndex) {
        aPageIndex = Mathf.Clamp(aPageIndex, 0, _pageCount - 1);
        container.anchoredPosition = _pagePositions[aPageIndex];
        _currentPage = aPageIndex;
    }

    private void LerpToPage(int aPageIndex) {
        aPageIndex = Mathf.Clamp(aPageIndex, 0, _pageCount - 1);
        _lerpTo = _pagePositions[aPageIndex];
        _lerp = true;
        _currentPage = aPageIndex;
    }

    private void NextScreen() {
        LerpToPage(_currentPage + 1);
    }

    private void PreviousScreen() {
        LerpToPage(_currentPage - 1);
    }

    private int GetNearestPage() {
        // based on distance from current position, find nearest page
        Vector2 currentPosition = container.anchoredPosition;

        float distance = float.MaxValue;
        int nearestPage = _currentPage;

        for (int i = 0; i < _pagePositions.Count; i++) {
            float testDist = Vector2.SqrMagnitude(currentPosition - _pagePositions[i]);
            if (testDist < distance) {
                distance = testDist;
                nearestPage = i;
            }
        }

        return nearestPage;
    }

    public void OnBeginDrag(PointerEventData aEventData) {
        // if currently lerping, then stop it as user is draging
        _lerp = false;
        // not dragging yet
        _dragging = false;
    }

    public void OnEndDrag(PointerEventData aEventData) {
        // how much was container's content dragged
        float difference;
        
        difference = _startPosition.x - container.anchoredPosition.x;

        // test for fast swipe - swipe that moves only +/-1 item
        if (Time.unscaledTime - _timeStamp < fastSwipeThresholdTime &&
            Mathf.Abs(difference) > fastSwipeThresholdDistance &&
            Mathf.Abs(difference) < _fastSwipeThresholdMaxLimit) {
            if (difference > 0) {
                NextScreen();
            } else {
                PreviousScreen();
            }
        } else {
            // if not fast time, look to which page we got to
            LerpToPage(GetNearestPage());
        }

        _dragging = false;
    }

    public void OnDrag(PointerEventData aEventData) {
        if (!_dragging) {
            // dragging started
            _dragging = true;
            // save time - unscaled so pausing with Time.scale should not affect it
            _timeStamp = Time.unscaledTime;
            // save current position of cointainer
            _startPosition = container.anchoredPosition;
        }
    }
}
