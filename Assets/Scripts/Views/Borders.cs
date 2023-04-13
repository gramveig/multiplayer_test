using UnityEngine;

namespace AlexeyVlasyuk.MultiplayerTest.Views
{
    public class Borders : MonoBehaviour
    {
        [SerializeField]
        private float _colWidth = 4f;
        
        [SerializeField]
        private float _zPosition = 0f;
        
        public void Generate()
        {
            var topCollider = new GameObject().transform;
            var bottomCollider = new GameObject().transform;
            var rightCollider = new GameObject().transform;
            var leftCollider = new GameObject().transform;

            topCollider.name = "TopCollider";
            bottomCollider.name = "BottomCollider";
            rightCollider.name = "RightCollider";
            leftCollider.name = "LeftCollider";

            topCollider.gameObject.AddComponent<BoxCollider2D>();
            bottomCollider.gameObject.AddComponent<BoxCollider2D>();
            rightCollider.gameObject.AddComponent<BoxCollider2D>();
            leftCollider.gameObject.AddComponent<BoxCollider2D>();

            topCollider.gameObject.tag = "Border";
            bottomCollider.gameObject.tag = "Border";
            rightCollider.gameObject.tag = "Border";
            leftCollider.gameObject.tag = "Border";

            var t = transform;
            topCollider.parent = t;
            bottomCollider.parent = t;
            rightCollider.parent = t;
            leftCollider.parent = t;

            var cameraPos = Camera.main.transform.position;
            var screenSize = new Vector2(
                Vector2.Distance(Camera.main.ScreenToWorldPoint(new Vector2(0,0)),Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0))) * 0.5f,
                Vector2.Distance(Camera.main.ScreenToWorldPoint(new Vector2(0,0)),Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height))) * 0.5f
            );
            
            rightCollider.localScale = new Vector3(_colWidth, screenSize.y * 2, _colWidth);
            rightCollider.position = new Vector3(cameraPos.x + screenSize.x + (rightCollider.localScale.x * 0.5f), cameraPos.y, _zPosition);
            leftCollider.localScale = new Vector3(_colWidth, screenSize.y * 2, _colWidth);
            leftCollider.position = new Vector3(cameraPos.x - screenSize.x - (leftCollider.localScale.x * 0.5f), cameraPos.y, _zPosition);
            topCollider.localScale = new Vector3(screenSize.x * 2, _colWidth, _colWidth);
            topCollider.position = new Vector3(cameraPos.x, cameraPos.y + screenSize.y + (topCollider.localScale.y * 0.5f), _zPosition);
            bottomCollider.localScale = new Vector3(screenSize.x * 2, _colWidth, _colWidth);
            bottomCollider.position = new Vector3(cameraPos.x, cameraPos.y - screenSize.y - (bottomCollider.localScale.y * 0.5f), _zPosition);
        }
    }
}