using System.Collections;
using Assets.Scripts.StaticData;
using UnityEngine;
using UnityEngine.UI;

public class KilledCell : MonoBehaviour
{
	[HideInInspector] public bool IsFalling;
	[SerializeField] private float _speed = 16f;
	[SerializeField] private float _gravit = 32f;
	[SerializeField] private RectTransform _rect;
	[SerializeField] private Image _image;
	private Vector2 _moveDirection;

	public void Initialize(Sprite sprite, Vector2 startPosition)
	{
		IsFalling = true;

		_moveDirection = Vector2.up;
		_moveDirection.x = Random.Range(-1f, 1f);
		_moveDirection *= _speed / 2;

		_image.sprite = sprite;
		_rect.anchoredPosition = startPosition;

		StartCoroutine(WaitForDeath());
	}

	private IEnumerator WaitForDeath()
	{
		yield return new WaitForSeconds(5f);
		gameObject.SetActive(false);
		//Destroy(gameObject);
	}

	private void Update()
	{
		if (!IsFalling)
		{
			return;
		}

		_moveDirection.y -= Time.deltaTime * _gravit;
		_moveDirection.x = Mathf.Lerp(_moveDirection.x, 0, Time.deltaTime);

		_rect.anchoredPosition += _moveDirection * (Time.deltaTime * _speed);

		if (_rect.position.x < -Config.PIECE_SIZE
		    || _rect.position.x > Screen.width * Config.PIECE_SIZE
		    || _rect.position.y < -Config.PIECE_SIZE
		    || _rect.position.y > Screen.height * Config.PIECE_SIZE)
		{
			IsFalling = false;
		}
	}
}