using static MetaBalls.Constants;
using SFML.Window;
using SFML.Graphics;
using SFML.System;
using System.Numerics;

namespace MetaBalls;

internal class WindowSFML
{
	private byte[] BUFFER = new byte[RENDER_H*RENDER_W*4];
	private Random RANDOM = new Random();
	private List<Circle> METABALLS = new List<Circle>();
	
	public void Run()
	{
		var videoMode = new VideoMode(WINDOW_WIDTH, WINDOW_HEIGHT);
		var window = new RenderWindow(videoMode, "Hello MetaBalls");
		window.SetFramerateLimit(30);
		window.SetVerticalSyncEnabled(true);
		window.KeyPressed += Window_KeyPressed;

		string fontPath = Path.Combine(
			AppDomain.CurrentDomain.BaseDirectory,
			"Fonts/open-sans/OpenSans-Regular.ttf"
		);
		Font font = new Font(fontPath);
		
		//FPS
		var clock = new Clock();
		float fps = 0.0f;
		Text fpsText = new Text()
		{
			Font = font
		};
		
		//Texture & Sprite
		Texture tex = new Texture(RENDER_W, RENDER_H);
		Sprite sprite = new Sprite(tex);
		sprite.Scale = new Vector2f
		{
			X = (float)WINDOW_WIDTH / RENDER_W,
			Y = (float)WINDOW_HEIGHT / RENDER_H
		};
		
		for (int i = 0; i < 6; i++)
		{
			var circle = new Circle(new Vector2(0.0f, 0.0f), 0.06f);
			circle.DirectionX = RANDOM.Next(-5, 6) / 500.0f;
			circle.DirectionY = RANDOM.Next(-5, 6) / 500.0f;
			METABALLS.Add(circle);
		}

		while (window.IsOpen)
		{
			//Setup
			window.DispatchEvents();
			clock.Restart();
			window.Clear();
			Array.Fill<byte>(BUFFER, 0);
			
			//Byte Array
			Parallel.For(0, RENDER_H, new ParallelOptions {MaxDegreeOfParallelism = 3}, (row) => 
			{
				for(int col = 0; col < RENDER_W; col++)
				{
					DrawMetaBall(row, col);
				}
			});
			
			//Draw
			tex.Update(BUFFER);
			window.Draw(sprite);
			window.Draw(fpsText);
			window.Display();
			
			// Update MetaBalls
			foreach (Circle metaBall in METABALLS)
			{
				UpdateMetaBall(metaBall);
			}
			
			//Fps
			fps = 1.0f / clock.ElapsedTime.AsSeconds();
			fpsText.DisplayedString = $"FPS: {fps:0.00}";
		}
	}
	
	private void DrawMetaBall(int row, int col)
	{
		var pixNormX = WINDOW_ASPECT * ((col / ((float)RENDER_W / 2)) - 1);
		var pixNormY = (row / ((float)RENDER_H / 2)) - 1;
		
		float sum = 0.0f;
		
		for (int i = 0; i < METABALLS.Count; i++)
		{
			float insideSqrt = (float)Math.Pow(pixNormX - METABALLS[i].PositionX, 2) + (float)Math.Pow(pixNormY - METABALLS[i].PositionY, 2);
			float distance = (float)Math.Sqrt(insideSqrt);
			sum += METABALLS[i].Radius / distance;
		}
		
		sum = 1 - sum;
		
		Color color;
		if (sum <= 0.02f)
			color = Color.White;
		else if (sum <= 0.15f)
			color = Color.Red;
		else if (sum <= 0.25f)
			color = Color.Yellow;
		else if (sum <= 0.35f)
			color = Color.Green;
		else if (sum <= 0.40)
			color = Color.Blue;
		else
			color = Color.Black;
		
		BUFFER[(row * RENDER_W * 4) + (col * 4) + 0] = color.R;
		BUFFER[(row * RENDER_W * 4) + (col * 4) + 1] = color.G;
		BUFFER[(row * RENDER_W * 4) + (col * 4) + 2] = color.B;
		BUFFER[(row * RENDER_W * 4) + (col * 4) + 3] = color.A;
	}
	
	private void UpdateMetaBall(Circle metaBall)
	{
		if (
			(metaBall.PositionX >= 1.0f && metaBall.DirectionX > 0.0f) ||
			(metaBall.PositionX <= -1.0f && metaBall.DirectionX < 0.0f)
		)
		{
			metaBall.DirectionX = -metaBall.DirectionX;
		}
		if (
			(metaBall.PositionY >= 1.0f && metaBall.DirectionY > 0.0f) ||
			(metaBall.PositionY <= -1.0f && metaBall.DirectionY < 0.0f)
		)
		{
			metaBall.DirectionY = -metaBall.DirectionY;
		}
		
		metaBall.PositionX += metaBall.DirectionX;
		metaBall.PositionY += metaBall.DirectionY;
	}
	
	private void Window_KeyPressed(object sender, KeyEventArgs eventArgs)
	{
		var window = (Window)sender;
		if (eventArgs.Code == Keyboard.Key.Escape)
			window.Close();
	}
}