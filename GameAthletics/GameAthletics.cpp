#include <SFML/Graphics.hpp>
#include <SFML/Audio.hpp>
#include "map.h"
#include "view.h"
#include "map_train.h"
#include <sstream>
#include <iomanip>
#include <fstream>
#include <iostream>

using namespace sf;
using namespace std;

int record_time;
int score;
bool isAudio = true;
bool isCloud = true;
bool GameTrain = true;
bool PauseTrain = true;
float SpeedPlayer = 0.13;

class Entity {
public:
	float dx, dy, x, y, speed, moveTimer; 
	int w, h;
	Texture texture;
	Sprite sprite;
	bool life;
	String name;
	Entity(Image& image, String Name, float X, float Y, int W, int H) {
		x = X; y = Y; w = W; h = H; 
		speed = 0; life = false; dx = 0; dy = 0;
		name = Name;
		texture.loadFromImage(image);
		sprite.setTexture(texture);
	}
};

class Player_Train :public Entity {
public:
	bool onGround;
	enum { up, down, jump, stay } state;//добавляем тип перечисления - состояние объекта
	float CurrFrame = 0;
	Player_Train(Image& image, String Name, float X, float Y, int W, int H) :Entity(image, Name, X, Y, W, H) {
		state = down;
		onGround = false;
		sprite.setTextureRect(IntRect(85, 1, w, h));
	}

	void control(float time) {
		if (!GameTrain )
			sprite.setTextureRect(IntRect(360, 350, 130, 85));
		if (!PauseTrain && GameTrain && !life && onGround) {
			state = stay;
			CurrFrame += 0.005 * time;
			if (CurrFrame > 4) CurrFrame -= 4;
			sprite.setTextureRect(IntRect(77 * int(CurrFrame) + 10, 0, 67, 131));
		}
		if (onGround && GameTrain && PauseTrain) {
			state = down;
			CurrFrame += 0.005 * time;
			if (CurrFrame > 6) CurrFrame -= 6;
			sprite.setTextureRect(IntRect(94 * int(CurrFrame), 172, 94, 128));
		}
		if (life && GameTrain && PauseTrain) {
			if (Keyboard::isKeyPressed) {//если нажата клавиша
				if ((Keyboard::isKeyPressed(Keyboard::Up)) && (onGround)) {//если нажата клавиша вверх и мы на земле, то можем прыгать
					sprite.setTextureRect(IntRect(10, 320, 85, 143));
					state = jump; dy = -0.75; onGround = false;//увеличил высоту прыжка
				}
			}
		}
	}

	void checkCollisionWithMap(float Dx, float Dy)//ф-ция проверки столкновений с картой
	{
		for (int i = y / 32; i < (y + h) / 32; i++)//проходимся по элементам карты
			for (int j = x / 32; j < (x + w) / 32; j++)
			{
				if (stadium_train[i][j] == 'd')//если элемент наш тайлик земли? то
				{
					if (Dy > 0) { y = i * 32 - h;  dy = 0; onGround = true; }//по Y вниз=>идем в пол(стоим на месте) или падаем. В этот момент надо вытолкнуть персонажа и поставить его на землю, при этом говорим что мы на земле тем самым снова можем прыгать
					if (Dy < 0) { y = i * 32 + 32;  dy = 0; }//столкновение с верхними краями карты(может и не пригодиться)
				}
				//else { onGround = false; }//надо убрать т.к мы можем находиться и на другой поверхности или платформе которую разрушит враг
			}
	}

	void update(float time)
	{

		control(time);//функция управления персонажем
		switch (state) {//тут делаются различные действия в зависимости от состояния
		case up: break;//будет состояние поднятия наверх (например по лестнице)
		case down: break;//будет состояние во время спуска персонажа (например по лестнице)
		case stay: break;//и здесь тоже	
		case jump: break;
		}
		x += dx * time;
		checkCollisionWithMap(dx, 0);//обрабатываем столкновение по Х
		if (GameTrain) {
			y += dy * time;
			checkCollisionWithMap(0, dy);
		}//обрабатываем столкновение по Y
		if (!GameTrain) sprite.setPosition(1650, 780);
		else sprite.setPosition(x, y);  //задаем позицию спрайта в место его центра
		getPlayerXYForView(x, y); 
		if (GameTrain) dy = dy + 0.0015 * time;//постоянно притягиваемся к земле
	}
};

class Player : public Entity {
public:
	float spavnY;
	enum { left, right, stay} state;
	float CurrFrame = 0;
	Player(Image& image, String Name, float X, float Y, int W, int H) : Entity(image, Name, X, Y, W, H) {
		spavnY = Y;
		state = stay;
		if (name == "my_Player") {
			sprite.setTextureRect(IntRect(0, 0, w, h));
		}
	}
	void control(float time) {
		state = stay;
		CurrFrame += 0.005 * time;
		if (CurrFrame > 4) CurrFrame -= 4;
		sprite.setTextureRect(IntRect(77 * int(CurrFrame) + 10, 0, 67, 131));

		if (life) {
			if (Keyboard::isKeyPressed) {
				if (Keyboard::isKeyPressed(Keyboard::Right)) {
					state = right;
					speed = SpeedPlayer;
					if (Keyboard::isKeyPressed(Keyboard::Down)) {
						dy = 0.03;
					}
					if (Keyboard::isKeyPressed(Keyboard::Up)) {
						dy = -0.03;
					}
					CurrFrame += 0.005 * time;
					if (CurrFrame > 6) CurrFrame -= 6;
					sprite.setTextureRect(IntRect(94 * int(CurrFrame), 172, 94, 128));
				}
				if (Keyboard::isKeyPressed(Keyboard::Left)) {
					state = left;
					speed = SpeedPlayer;
					if (Keyboard::isKeyPressed(Keyboard::Down)) {
						dy = 0.03;
					}
					if (Keyboard::isKeyPressed(Keyboard::Up)) {
						dy = -0.03;
					}
					CurrFrame += 0.005 * time;
					if (CurrFrame > 6) CurrFrame -= 6;
					sprite.setTextureRect(IntRect(94 * int(CurrFrame) + 94, 172, -94, 128));
				}

			}
		}
	}
	void checkCollisionWithMap(float Dx, float Dy) {
		for (int i = y / 32; i < (y + h) / 32; i++) 
			for (int j = x / 32; j < (x + w) / 32; j++) {
				if (my_stadium[i][j] == 's' ) {
					if (Dx > 0) { x = j * 32 - w; }//с правым краем карты
					if (Dx < 0) { x = j * 32 + 32; }// с левым краем карты
				}

				if ((spavnY - y) > 25) { y = spavnY - 25; dy = 0; }
				if ((y - spavnY) > 25) { y = spavnY + 25; dy = 0; }
				
			}
	}
	void update(float time) {
		control(time);
		switch (state) {
		case right: dx = speed; break;
		case left: dx = -speed; break;
		case stay: dx = 0.00000000001; dy = 0.000000001; break;
		}
		x += dx * time;
		checkCollisionWithMap(dx, 0);
		y += dy * time;
		checkCollisionWithMap(0, dy); 
		
		sprite.setPosition(x, y);
		getPlayerXYForView(x, y);
	}

};

class Enemy :public Entity {
public:
	float CurrFrame = 0;
	float CurrFrameTribuns = 0;
	Enemy(Image& image, String Name, float X, float Y, int W, int H) :Entity(image, Name, X, Y, W, H) {
		if (name == "cloud" || name == "cloud_train") {
			sprite.setTextureRect(IntRect(0, 0, w, h));
			dx = 0.05;//даем скорость.этот объект всегда двигается
			life = true;
		}
		if (name == "easyEnemy" || name == "hardEnemy" ) {
			sprite.setTextureRect(IntRect(0, 0, w, h));
		}
		if (name == "tribuns") {
			sprite.setTextureRect(IntRect(0, 0, w, h));
			life = true;
		}
		if (name == "barier") {
			sprite.setTextureRect(IntRect(0, 0, w, h));
			dx = 0.2;
		}

	}

	void checkCollisionWithMap(float Dx, float Dy)//ф ция проверки столкновений с картой
	{
		for (int i = y / 32; i < (y + h) / 32; i++)//проходимся по элементам карты
			for (int j = x / 32; j < (x + w) / 32; j++)
			{
				if (my_stadium[i][j] == 's')//если элемент наш тайлик земли, то
				{
					if (name == "cloud") {
						if (Dx > 0) { x = j * 32 - w; dx = -0.05; }//с правым краем карты
						if (Dx < 0) { x = j * 32 + 32; dx = 0.05; }// с левым краем карты
					}
				}
				if (name == "easyEnemy")
					if (x >= 3090) life = false;
				if (name == "hardEnemy")
					if (x >= 3150) life = false;
 			}
	}

	void control(float time) {
		dx = 0;
		CurrFrame += 0.005 * time;
		if (CurrFrame > 4) CurrFrame -= 4;
		sprite.setTextureRect(IntRect(77 * int(CurrFrame) + 10, 0, 67, 131));
		if (life) {
			if (name == "easyEnemy") dx = 0.13;
			if (name == "hardEnemy") dx = 0.145;
			CurrFrame += 0.0047 * time;
			if (CurrFrame > 6) CurrFrame -= 6;
			sprite.setTextureRect(IntRect(94 * int(CurrFrame), 172, 94, 128));
		}
	}

	void update(float time)
	{
		if (name == "cloud" && life == true) {//для персонажа с таким именем логика будет такой
			x += dx * time;
			checkCollisionWithMap(dx, 0);
			sprite.setPosition(x, y);  //задаем позицию спрайта в место его центра
		}
		if (name == "easyEnemy" || name == "hardEnemy") {
			control(time);
			x += dx * time;
			checkCollisionWithMap(dx, 0);
			sprite.setPosition(x, y);
		}
		if (name == "tribuns" && life == true) {
			CurrFrameTribuns += 0.007 * time;
			if (CurrFrameTribuns > 4) CurrFrameTribuns -= 4;
			sprite.setTextureRect(IntRect(0, 200 * int(CurrFrameTribuns), 500, 200));
			sprite.setPosition(x, y);
		}
		if (name == "cloud_train" && life == true) {
			x -= dx * time;
			if (x <= 670) x = 2500;
			sprite.setPosition(x, y);
		}
		if (name == "barier" && life == true) {
			x -= dx * time;
			if (x <= 845) x = 2415;
			sprite.setPosition(x, y);
		}
	}
};

bool Level_Train(RenderWindow& window) {
	view.reset(FloatRect(0, 0, 1500, 700));

	Music menuMusic;
	menuMusic.openFromFile("audio/menu.ogg");
	menuMusic.setVolume(5.0f);

	if (isAudio) {
		menuMusic.play();
		menuMusic.setLoop(true);
	}

	Font font;
	font.loadFromFile("Never_smile.ttf");
	Text text("", font, 90);
	text.setStyle(Text::Bold);
	Text textFinish("", font, 70);
	textFinish.setStyle(Text::Bold);

	Image image_player;
	image_player.loadFromFile("images/myRunner.png");
	image_player.createMaskFromColor(Color(255, 174, 201));
	
	Texture texture_map;
	texture_map.loadFromFile("images/stadium.png");
	Sprite sprite_map(texture_map);

	Image image_pause;
	image_pause.loadFromFile("images/button_pause.png");
	image_pause.createMaskFromColor(Color(255, 174, 201));
	Texture texture_pause;
	texture_pause.loadFromImage(image_pause);
	Sprite sprite_pause;
	sprite_pause.setTexture(texture_pause);
	sprite_pause.setTextureRect(IntRect(0, 0, 50, 50));
	sprite_pause.setScale(1.6f, 1.6f);

	Image image_button;
	image_button.loadFromFile("images/Continue_Exit.png");
	image_button.createMaskFromColor(Color(255, 174, 201));
	Texture texture_lobby_pause, texture_button_pause;
	texture_lobby_pause.loadFromFile("images/Lobby_Pause.png");
	texture_button_pause.loadFromImage(image_button);
	Sprite sprite_lobby_pause(texture_lobby_pause), sprite_pause_continue(texture_button_pause), sprite_pause_exit(texture_button_pause);
	sprite_lobby_pause.setTextureRect(IntRect(0, 0, 225, 168));
	sprite_pause_continue.setTextureRect(IntRect(0, 0, 110, 30));
	sprite_pause_exit.setTextureRect(IntRect(0, 30, 110, 30));
	sprite_lobby_pause.setScale(2.6f, 2.6f);
	sprite_pause_continue.setScale(2.5f, 2.2f);
	sprite_pause_exit.setScale(2.2f, 2.2f);

	Image image_cloud;
	image_cloud.loadFromFile("images/Cloud.png");
	image_cloud.createMaskFromColor(Color(255, 174, 201));

	Image image_barier;
	image_barier.loadFromFile("images/Barier.png");
	image_barier.createMaskFromColor(Color(255, 174, 201));

	Texture texture_startConsole;
	texture_startConsole.loadFromFile("images/startConsol.png");
	Sprite sprite_startConsole(texture_startConsole), sprite_finishConsole(texture_startConsole);
	sprite_finishConsole.setTextureRect(IntRect(0, 0, 300, 90));
	sprite_finishConsole.setScale(2.0f, 1.5f);
	sprite_startConsole.setTextureRect(IntRect(0, 0, 300, 90));

	Player_Train player(image_player, "my_Player", 1650, 731, 79, 134);

	Enemy cloud1(image_cloud, "cloud_train", 2500, 400, 248, 80);
	Enemy cloud2(image_cloud, "cloud_train", 1800, 550, 248, 80);

	Enemy barier(image_barier, "barier", 2415, 730, 10, 125);
	
	int start_time = 5;
	int difference = 0;

	Clock clock;
	Clock Game_time;
	int Run_time = 0;
	int Finish_time = 0;
	int timer_of_running = 0;
	int Pause_time = 0;
	int new_score = 0;

	bool get_score = true;
	bool pause = false;
	bool finish = false;
	bool isPause = false;

	PauseTrain = true;
	GameTrain = true;

	while (window.isOpen()) {
		Event event;

		float time = clock.getElapsedTime().asMicroseconds();

		Run_time = Game_time.getElapsedTime().asSeconds();

		time /= 800;
		clock.restart();

		Vector2i pixelPos = Mouse::getPosition(window);
		Vector2f pos = window.mapPixelToCoords(pixelPos);

		if ((Run_time - difference) >= start_time && !pause && !finish) {
			player.life = true;
			barier.life = true;
		}

		while (window.pollEvent(event)) {
			if (event.type == Event::Closed)
				window.close();
		}
		if (Keyboard::isKeyPressed(Keyboard::Tab)) { return false; }

		if (isCloud) {
			cloud1.update(time);
			cloud2.update(time);
		}

		barier.update(time);

		player.update(time);
		window.setView(view);

		window.clear();

		for (int i = 0; i < HEIGHT_MAP_TRAIN; i++)
			for (int j = 0; j < WIDTH_MAP_TRAIN; j++) {
				if (stadium_train[i][j] == '0') sprite_map.setTextureRect(IntRect(192, 0, 32, 32));
				if (stadium_train[i][j] == 's') sprite_map.setTextureRect(IntRect(224, 0, 32, 32));
				if (stadium_train[i][j] == 'f') sprite_map.setTextureRect(IntRect(128, 0, 32, 32));
				if (stadium_train[i][j] == 'b') sprite_map.setTextureRect(IntRect(160, 0, 32, 32));
				if (stadium_train[i][j] == 'u') sprite_map.setTextureRect(IntRect(0, 0, 32, 32));
				if (stadium_train[i][j] == 'd') sprite_map.setTextureRect(IntRect(32, 0, 32, 32));

				sprite_map.setPosition(j * 32, i * 32);
				window.draw(sprite_map);
			}
		sprite_pause.setPosition(view.getCenter().x - 720, view.getCenter().y - 325);
		//cout << player.x << " " << player.y << endl;

		if (barier.x >= 1647 && barier.x <= 1670 && player.y >= 630 && !finish) {
			finish = true;
			timer_of_running = Run_time - start_time - difference;
			Finish_time = Run_time - difference; 
			GameTrain = false;
		}
		
		//cout << player.y << " " << barier.x << endl;
		
		if (isCloud) {
			window.draw(cloud1.sprite);
			window.draw(cloud2.sprite);
		}

		window.draw(barier.sprite);

		window.draw(player.sprite);

		window.draw(sprite_pause);

		ostringstream start_time_str;
		if (start_time - (Run_time - difference) >= 0) {
			start_time_str << (start_time - (Run_time - difference));
			if (start_time - (Run_time - difference) > 0) {
				text.setString(start_time_str.str());
				text.setPosition(view.getCenter().x - 35, view.getCenter().y - 20);
			}
			else {
				text.setString("START!");
				text.setPosition(view.getCenter().x - 125, view.getCenter().y - 20);
			}
			sprite_startConsole.setPosition(view.getCenter().x - 170, view.getCenter().y - 5);
			window.draw(sprite_startConsole);
			window.draw(text);
		}

		ostringstream Runner_Score;
		if (finish) {
			if (get_score) {
				new_score = timer_of_running * 15;
				score += new_score;
				ofstream files("score.txt");
				files << score;
				files.close();
				get_score = false;
			}

			if (((Run_time - difference) - Finish_time) >= 0) {
				player.life = false;
				barier.life = false;
				Runner_Score << new_score;
				textFinish.setString("Score: " + Runner_Score.str());
				textFinish.setPosition(view.getCenter().x - 90, view.getCenter().y + 10);
				sprite_finishConsole.setPosition(view.getCenter().x - 250, view.getCenter().y);

				window.draw(sprite_finishConsole);
				window.draw(textFinish);
			}
			if (((Run_time - difference) - Finish_time) == 5) { return false; }
		}

		if (sprite_pause.getGlobalBounds().contains(pos.x, pos.y) && Mouse::isButtonPressed(Mouse::Left)) { pause = true; Pause_time = (Run_time - difference); }
		if (pause) {
			isPause = true;
			PauseTrain = false;
		}
		//cout << Run_time << " " << Pause_time << " " << difference << endl;
		if (isPause) {
			cloud1.life = false; cloud2.life = false;  player.life = false; barier.life = false;
			sprite_lobby_pause.setPosition(view.getCenter().x - 300, view.getCenter().y - 200);
			sprite_pause_continue.setPosition(view.getCenter().x - 145, view.getCenter().y - 80);
			sprite_pause_exit.setPosition(view.getCenter().x - 125, view.getCenter().y + 40);
			window.draw(sprite_lobby_pause);
			window.draw(sprite_pause_continue);

			if (sprite_pause_continue.getGlobalBounds().contains(pos.x, pos.y) && Mouse::isButtonPressed(Mouse::Left)) {
				isPause = false;
				pause = false;
				cloud1.life = true; cloud2.life = true; 
				PauseTrain = true;

				difference += ((Run_time - difference) - Pause_time);
				Pause_time = 0;

				if ((Run_time - difference) >= start_time && !finish && !pause) { barier.life = true; player.life = true; }

			}

			window.draw(sprite_pause_exit);
			if (event.type == Event::MouseButtonPressed) {
				if (event.key.code == Mouse::Left)
					if (sprite_pause_exit.getGlobalBounds().contains(pos.x, pos.y)) { return false; }
			}

		}
		window.display();
	}
}

bool Level_Game_1(RenderWindow& window) {
	view.reset(FloatRect(0, 0, 1500, 700));

	Music musicTribuns, menuMusic;
	musicTribuns.openFromFile("audio/fanats.ogg");
	menuMusic.openFromFile("audio/menu.ogg");
	musicTribuns.setVolume(15.0f);
	menuMusic.setVolume(5.0f);
	
	if (isAudio) {
		menuMusic.play();
		musicTribuns.play();
		menuMusic.setLoop(true);
		musicTribuns.setLoop(true);
	}

	SoundBuffer winBuffer, loseBuffer;
	winBuffer.loadFromFile("audio/victory.ogg");
	loseBuffer.loadFromFile("audio/lose.ogg");
	Sound soundWin(winBuffer);
	soundWin.setVolume(25.0f);
	Sound soundLose(loseBuffer);
	soundLose.setVolume(35.0f);

	Font font;
	font.loadFromFile("Never_smile.ttf");
	Text text("", font, 90);
	text.setStyle(Text::Bold);
	Text textFinish("", font, 70);
	textFinish.setStyle(Text::Bold);

	Texture texture_map;
	texture_map.loadFromFile("images/stadium.png");
	Sprite sprite_map(texture_map);

	Image image_pause;
	image_pause.loadFromFile("images/button_pause.png");
	image_pause.createMaskFromColor(Color(255, 174, 201));
	Texture texture_pause;
	texture_pause.loadFromImage(image_pause);
	Sprite sprite_pause;
	sprite_pause.setTexture(texture_pause);
	sprite_pause.setTextureRect(IntRect(0, 0, 50, 50));
	sprite_pause.setScale(1.6f, 1.6f);

	Image image_button;
	image_button.loadFromFile("images/Continue_Exit.png");
	image_button.createMaskFromColor(Color(255, 174, 201));
	Texture texture_lobby_pause, texture_button_pause;
	texture_lobby_pause.loadFromFile("images/Lobby_Pause.png");
	texture_button_pause.loadFromImage(image_button);
	Sprite sprite_lobby_pause(texture_lobby_pause), sprite_pause_continue(texture_button_pause), sprite_pause_exit(texture_button_pause);
	sprite_lobby_pause.setTextureRect(IntRect(0, 0, 225, 168));
	sprite_pause_continue.setTextureRect(IntRect(0, 0, 110, 30));
	sprite_pause_exit.setTextureRect(IntRect(0, 30, 110, 30));
	sprite_lobby_pause.setScale(2.6f, 2.6f);
	sprite_pause_continue.setScale(2.5f, 2.2f);
	sprite_pause_exit.setScale(2.2f, 2.2f);

	Image image_player;
	image_player.loadFromFile("images/myRunner.png");
	image_player.createMaskFromColor(Color(255, 174, 201));

	Image image_enemy;
	image_enemy.loadFromFile("images/EnemyRunner.png");
	image_enemy.createMaskFromColor(Color(255, 174, 201));

	Image image_cloud;
	image_cloud.loadFromFile("images/Cloud.png");
	image_cloud.createMaskFromColor(Color(255, 174, 201));

	Image image_tribuns;
	image_tribuns.loadFromFile("images/Tribuns.png");
	image_tribuns.createMaskFromColor(Color(255, 174, 201));

	Texture texture_finish_list;
	texture_finish_list.loadFromFile("images/OlympicVector.jpg");
	Sprite sprite_finish_list(texture_finish_list);
	sprite_finish_list.setTextureRect(IntRect(0, 0, 400, 300));
	sprite_finish_list.setScale(2.1f, 2.1f);

	Texture texture_startConsole;
	texture_startConsole.loadFromFile("images/startConsol.png");
	Sprite sprite_startConsole(texture_startConsole);
	sprite_startConsole.setTextureRect(IntRect(0, 0, 300, 90));

	Enemy cloud1(image_cloud, "cloud", 1700, 310, 248, 80);
	Enemy cloud2(image_cloud, "cloud", 50, 180, 248, 80);
	Enemy cloud3(image_cloud, "cloud", 3030, 220, 248, 80);
	Enemy cloud4(image_cloud, "cloud", 2500, 130, 248, 80);
	Enemy cloud5(image_cloud, "cloud", 1000, 280, 248, 80);

	Enemy tribuns1(image_tribuns, "tribuns", 800, 370, 500, 200);
	Enemy tribuns2(image_tribuns, "tribuns", 1600, 370, 500, 200);
	Enemy tribuns3(image_tribuns, "tribuns", 2400, 370, 500, 200);

	Enemy enemy1(image_enemy, "easyEnemy", 96, 545, 79, 134);
	Enemy enemy2(image_enemy, "hardEnemy", 100, 737, 79, 134);
	Enemy enemy3(image_enemy, "easyEnemy", 98, 673, 79, 134);
	
	Player player(image_player, "my_Player", 100, 609, 79, 134);

	int start_time = 5;
	int finish_time = 10;
	int difference = 0;

	Clock clock;
	Clock Game_time;
	int Run_time = 0;
	int Finish_time = 0;
	int timer_of_running = 0;
	int Pause_time = 0;

	bool finish = false;
	bool victory = true;
	bool newRecord = false;
	bool pause = false;
	bool isPause = false;

	while (window.isOpen()) {
		Event event;

		float time = clock.getElapsedTime().asMicroseconds();

		Run_time = Game_time.getElapsedTime().asSeconds();

		if ((Run_time - difference) >= start_time && !finish && victory && !pause) {
			player.life = true;
			enemy1.life = true;
			enemy2.life = true;
			enemy3.life = true;
		}

		clock.restart();
		time /= 800;

		Vector2i pixelPos = Mouse::getPosition(window);
		Vector2f pos = window.mapPixelToCoords(pixelPos);

		while (window.pollEvent(event)) {
			if (event.type == Event::Closed) { window.close(); }

		}
		if (Keyboard::isKeyPressed(Keyboard::Tab)) { return false; }
		
		if (isCloud) {
			cloud1.update(time);
			cloud2.update(time);
			cloud3.update(time);
			cloud4.update(time);
			cloud5.update(time);
		}

		tribuns1.update(time);
		tribuns2.update(time);
		tribuns3.update(time);

		enemy1.update(time);
		enemy2.update(time);
		enemy3.update(time);

		player.update(time);
		window.setView(view);
		window.clear();

		for (int i = 0; i < HEIGHT_MAP; i++)
			for (int j = 0; j < WIDTH_MAP; j++) {
				if (my_stadium[i][j] == '0') sprite_map.setTextureRect(IntRect(192, 0, 32, 32));
				if (my_stadium[i][j] == 's') sprite_map.setTextureRect(IntRect(224, 0, 32, 32));
				if (my_stadium[i][j] == 'f') sprite_map.setTextureRect(IntRect(128, 0, 32, 32));
				if (my_stadium[i][j] == 'b') sprite_map.setTextureRect(IntRect(160, 0, 32, 32));
				if (my_stadium[i][j] == 'u') sprite_map.setTextureRect(IntRect(0, 0, 32, 32));
				if (my_stadium[i][j] == 'd') sprite_map.setTextureRect(IntRect(32, 0, 32, 32));
				if (my_stadium[i][j] == '1') sprite_map.setTextureRect(IntRect(64, 0, 32, 32));
				if (my_stadium[i][j] == '2') sprite_map.setTextureRect(IntRect(96, 0, 32, 32));
				if (my_stadium[i][j] == 'q') sprite_map.setTextureRect(IntRect(0, 32, 32, 32));
				if (my_stadium[i][j] == 'w') sprite_map.setTextureRect(IntRect(32, 32, 32, 32));
				if (my_stadium[i][j] == 'e') sprite_map.setTextureRect(IntRect(0, 64, 32, 32));
				if (my_stadium[i][j] == 'r') sprite_map.setTextureRect(IntRect(32, 64, 32, 32));
				if (my_stadium[i][j] == 't') sprite_map.setTextureRect(IntRect(64, 32, 32, 32));
				if (my_stadium[i][j] == 'y') sprite_map.setTextureRect(IntRect(96, 32, 32, 32));
				if (my_stadium[i][j] == 'o') sprite_map.setTextureRect(IntRect(64, 64, 32, 32));
				if (my_stadium[i][j] == 'i') sprite_map.setTextureRect(IntRect(96, 64, 32, 32));
				if (my_stadium[i][j] == 'a') sprite_map.setTextureRect(IntRect(128, 32, 32, 32));
				if (my_stadium[i][j] == 'n') sprite_map.setTextureRect(IntRect(160, 32, 32, 32));
				if (my_stadium[i][j] == 'h') sprite_map.setTextureRect(IntRect(128, 64, 32, 32));
				if (my_stadium[i][j] == 'j') sprite_map.setTextureRect(IntRect(160, 64, 32, 32));
				if (my_stadium[i][j] == 'k') sprite_map.setTextureRect(IntRect(192, 32, 32, 32));
				if (my_stadium[i][j] == 'l') sprite_map.setTextureRect(IntRect(224, 32, 32, 32));
				if (my_stadium[i][j] == 'z') sprite_map.setTextureRect(IntRect(192, 64, 32, 32));
				if (my_stadium[i][j] == 'x') sprite_map.setTextureRect(IntRect(224, 64, 32, 32));

				sprite_map.setPosition(j * 32, i * 32);
				window.draw(sprite_map);
			}
		sprite_pause.setPosition(view.getCenter().x - 720, view.getCenter().y - 325);
		if (isCloud) {
			window.draw(cloud1.sprite);
			window.draw(cloud2.sprite);
			window.draw(cloud3.sprite);
			window.draw(cloud4.sprite);
			window.draw(cloud5.sprite);
		}

		window.draw(tribuns1.sprite);
		window.draw(tribuns2.sprite);
		window.draw(tribuns3.sprite);

		window.draw(enemy1.sprite);
		window.draw(player.sprite);
		window.draw(enemy3.sprite);
		window.draw(enemy2.sprite);

		window.draw(sprite_pause);

		std::ostringstream start_time_str;
		if (start_time - (Run_time - difference) >= 0) {
			start_time_str << (start_time - (Run_time - difference));
			if (start_time - (Run_time - difference) > 0) {
				text.setString(start_time_str.str());
				text.setPosition(view.getCenter().x - 35, view.getCenter().y - 20);
			}
			else {
				text.setString("START!");
				text.setPosition(view.getCenter().x - 125, view.getCenter().y - 20);
			}
			sprite_startConsole.setPosition(view.getCenter().x - 170, view.getCenter().y - 5);
			window.draw(sprite_startConsole);
			window.draw(text);
		}

		if ((enemy1.x >= 3030 || enemy2.x >= 3030 || enemy3.x >= 3030) && !finish && victory)
			victory = false;

		if (player.x >= 3030 && !finish) {
			finish = true;
			timer_of_running = Run_time - start_time - difference;
			Finish_time = Run_time - difference;
		}
		
		ostringstream Runner_time_str_minuts, Runner_time_str_seconds, Runner_record_str_seconds, Runner_record_str_minuts;
		if (finish) {
			if (record_time > timer_of_running) {
				newRecord = true;
				record_time = timer_of_running;
				ofstream file("record_time.txt");
				file << record_time;
				file.close();
			}
			Runner_time_str_minuts << (timer_of_running / 60);
			Runner_time_str_seconds << (timer_of_running % 60);
			Runner_record_str_minuts << (record_time / 60);
			Runner_record_str_seconds << (record_time % 60);
			if (((Run_time - difference) - Finish_time) >= 3) {
				player.life = false;
				if (victory && newRecord) {
					textFinish.setString("You win!\nYour result: " + Runner_time_str_minuts.str() + ":" + Runner_time_str_seconds.str() + 
						"\nYour record: " + Runner_record_str_minuts.str() + ":" + Runner_record_str_seconds.str() + "\nYou have a new record!");
				}
				else if (victory && !newRecord) {
					textFinish.setString("You win!\nYour result: " + Runner_time_str_minuts.str() + ":" + Runner_time_str_seconds.str() +
						"\nYour record: " + Runner_record_str_minuts.str() + ":" + Runner_record_str_seconds.str());
				}
				else if (!victory && newRecord) {
					textFinish.setString("You lose!\nYour result: " + Runner_time_str_minuts.str() + ":" + Runner_time_str_seconds.str() +
						"\nYour record: " + Runner_record_str_minuts.str() + ":" + Runner_record_str_seconds.str() + "\nYou have a new record!");
				}
				else if (!victory && !newRecord) {
					textFinish.setString("You lose!\nYour result: " + Runner_time_str_minuts.str() + ":" + Runner_time_str_seconds.str() +
						"\nYour record: " + Runner_record_str_minuts.str() + ":" + Runner_record_str_seconds.str());
				}
				
				textFinish.setPosition(view.getCenter().x - 380, view.getCenter().y-310);
				sprite_finish_list.setPosition(view.getCenter().x - 425, view.getCenter().y - 315);

				window.draw(sprite_finish_list);
				window.draw(textFinish);
			}
			if (((Run_time - difference)- Finish_time) == 9) { return false; }
			if (isAudio) {
				if (((Run_time - difference) - Finish_time) == 0 && victory) soundWin.play();
				else if (((Run_time - difference) - Finish_time) == 0 && !victory) soundLose.play();
			}

		}
		
		if (sprite_pause.getGlobalBounds().contains(pos.x, pos.y) && Mouse::isButtonPressed(Mouse::Left)) { pause = true; Pause_time = (Run_time - difference); }
		if (pause) {
			isPause = true;
		}
		//cout << Run_time << " " << Pause_time << " " << difference << endl;
		if (isPause) {
			cloud1.life = false; cloud2.life = false; cloud3.life = false; cloud4.life = false; cloud5.life = false; player.life = false; musicTribuns.stop();
			tribuns1.life = false; tribuns2.life = false; tribuns3.life = false; enemy1.life = false; enemy2.life = false; enemy3.life = false;
			sprite_lobby_pause.setPosition(view.getCenter().x - 300, view.getCenter().y - 200);
			sprite_pause_continue.setPosition(view.getCenter().x - 145, view.getCenter().y - 80);
			sprite_pause_exit.setPosition(view.getCenter().x - 125, view.getCenter().y + 40);
			window.draw(sprite_lobby_pause);
			window.draw(sprite_pause_continue);

			if (sprite_pause_continue.getGlobalBounds().contains(pos.x, pos.y) && Mouse::isButtonPressed(Mouse::Left)) {
				isPause = false;
				pause = false;
				cloud1.life = true; cloud2.life = true; cloud3.life = true; cloud4.life = true; cloud5.life = true; 
				musicTribuns.play(); tribuns1.life = true; tribuns2.life = true; tribuns3.life = true;
				
				difference += ((Run_time - difference) - Pause_time);
				Pause_time = 0;

				if ((Run_time - difference) >= start_time && !finish && victory && !pause) { enemy1.life = true; enemy2.life = true; enemy3.life = true; player.life = true;  }

			}

			window.draw(sprite_pause_exit);
			if (event.type == Event::MouseButtonPressed) {
				if (event.key.code == Mouse::Left)
					if (sprite_pause_exit.getGlobalBounds().contains(pos.x, pos.y)) { return false; }
			}

		}
		window.display();
	}
}

bool Setting(RenderWindow& window) {
	view.reset(FloatRect(0, 0, 1500, 700));
	bool Set = true;

	Font font;
	font.loadFromFile("Never_smile.ttf");
	Text text("", font, 25);
	text.setStyle(Text::Bold);
	Text text_score("", font, 25);
	text.setStyle(Text::Bold);
	Text text_speed("", font, 25);
	text.setStyle(Text::Bold);

	Image image_plus;
	image_plus.loadFromFile("images/PlusForImprove.png");
	image_plus.createMaskFromColor(Color(255, 174, 201));
	Texture texture_plus;
	texture_plus.loadFromImage(image_plus);
	Sprite sprite_plus(texture_plus);

	Texture texture_Setting, texture_on_off;
	texture_Setting.loadFromFile("images/Setting_Lobby.png");
	texture_on_off.loadFromFile("images/On_Off.png");
	bool Num_Audio = false, Num_Cloud = false;
	Sprite sprite_Setting(texture_Setting), sprite_on_off_audio(texture_on_off), sprite_on_off_cloud(texture_on_off);
	bool improveScore = false;;
	while (Set) {
		sprite_on_off_audio.setColor(Color::White);
		sprite_on_off_cloud.setColor(Color::White);
		window.clear(Color::Cyan);

		sprite_plus.setTextureRect(IntRect(0, 0, 30, 30));

		if (isAudio) sprite_on_off_audio.setTextureRect(IntRect(0, 0, 80, 40));
		else if (!isAudio) sprite_on_off_audio.setTextureRect(IntRect(80, 0, 80, 40));

		if (isCloud) sprite_on_off_cloud.setTextureRect(IntRect(0, 0, 80, 40));
		else if (!isCloud) sprite_on_off_cloud.setTextureRect(IntRect(80, 0, 80, 40));

		sprite_plus.setPosition(200, 55);
		sprite_on_off_audio.setPosition(130, 95);
		sprite_on_off_cloud.setPosition(130, 145);

		if (IntRect(200, 55, 30, 30).contains(Mouse::getPosition(window)) && Mouse::isButtonPressed(Mouse::Left) && score >= 1000) improveScore = true;
		if (improveScore) {
			SpeedPlayer += 0.001;
			score -= 1000;
			ofstream file("score.txt");
			file << score;
			file.close();
			improveScore = false;
		}
		if (IntRect(130, 95, 80, 30).contains(Mouse::getPosition(window)) && Mouse::isButtonPressed(Mouse::Left)) Num_Audio = true;
		
		if (Num_Audio) {
			if (isAudio) {
				Num_Audio = false;
				isAudio = false;
			} 
			else if (!isAudio) {
				Num_Audio = false;
				isAudio = true;
			}
		}

		if (IntRect(130, 145, 80, 40).contains(Mouse::getPosition(window)) && Mouse::isButtonPressed(Mouse::Left)) Num_Cloud = true; 
		if (Num_Cloud) {
			if (isCloud) {
				Num_Cloud = false;
				isCloud = false;
			}
			else if (!isCloud) {
				Num_Cloud = false;
				isCloud = true;
			}
		}


		ostringstream Runner_time_str_minuts, Runner_time_str_seconds, Runner_Speed_Str, Runner_Score_Str;
		Runner_time_str_minuts << (record_time / 60);
		Runner_time_str_seconds << (record_time % 60);
		Runner_Speed_Str << (SpeedPlayer * 100);
		Runner_Score_Str << score;
		text.setString(Runner_time_str_minuts.str() + ":" + Runner_time_str_seconds.str());
		text_score.setString(Runner_Score_Str.str());
		text_speed.setString(Runner_Speed_Str.str());
		text.setPosition(110, 16);
		text_speed.setPosition(300, 16);
		text_score.setPosition(455, 17);
		

		if (Keyboard::isKeyPressed(Keyboard::Escape)) { return false; Set = false; }

		window.draw(sprite_Setting);
		window.draw(sprite_plus);
		window.draw(sprite_on_off_audio);
		window.draw(sprite_on_off_cloud);
		window.draw(text);
		window.draw(text_score);
		window.draw(text_speed);

		window.display();
	}
}

bool ChangeGame(RenderWindow& window) {
	view.reset(FloatRect(0, 0, 600, 400));

	Image imageLogo1, imageLogo2;
	imageLogo1.loadFromFile("images/CompetitionLogo.png");
	imageLogo2.loadFromFile("images/TrainerLogo.png");
	imageLogo1.createMaskFromColor(Color(255, 174, 201));
	imageLogo2.createMaskFromColor(Color(255, 174, 201));
	Texture textureLogo1, textureLogo2, textureViewNewGame;
	textureLogo1.loadFromImage(imageLogo1);
	textureLogo2.loadFromImage(imageLogo2);
	textureViewNewGame.loadFromFile("images/NewGameForChange.png");
	Sprite spriteViewNewGame(textureViewNewGame), spriteLogo1(textureLogo1), spriteLogo2(textureLogo2);
	spriteLogo1.setScale(1.2f, 1.2f);
	spriteLogo2.setScale(1.2f, 1.2f);
	spriteViewNewGame.setPosition(0, 0);
	spriteLogo1.setPosition(40, 50);
	spriteLogo2.setPosition(310, 50);

	bool Set = true;

	while (Set) {

		if (Keyboard::isKeyPressed(Keyboard::Escape)) {
			return false;
			Set = false;
		}

		if (IntRect(40, 50, 240, 290).contains(Mouse::getPosition(window)) && Mouse::isButtonPressed(Mouse::Left)) {
			if (Level_Game_1(window) == false) {
				return false;
				Set = false;
			}
		}
		if (IntRect(310, 50, 240, 290).contains(Mouse::getPosition(window)) && Mouse::isButtonPressed(Mouse::Left)) {
			if (Level_Train(window) == false) {
				return false;
				Set = false;
			}
		}

		window.draw(spriteViewNewGame);
		window.draw(spriteLogo1);
		window.draw(spriteLogo2);
		window.display();

	}
}

void Run_Game() {
	RenderWindow window(VideoMode(600, 400), "Run! Athletics");
	view.reset(FloatRect(0, 0, 1500, 700));

	Music menuMusic;
	menuMusic.openFromFile("audio/menu.ogg");
	menuMusic.setVolume(17.0f);
	if (isAudio) {
		menuMusic.play();
		menuMusic.setLoop(true);
	}

	Image imageBG;
	imageBG.loadFromFile("images/ChildBG.png");
	imageBG.createMaskFromColor(Color(255, 174, 201));
	Texture menuTexture1, menuTexture2, menuTexture3, menuTexture4, aboutTexture, menuBackground, Menu_Fon;
	menuTexture1.loadFromFile("images/New_Game.png");
	menuTexture2.loadFromFile("images/Setting.png");
	menuTexture3.loadFromFile("images/About_Game.png");
	menuTexture4.loadFromFile("images/Exit.png");
	aboutTexture.loadFromFile("images/AboutGame.png");
	Menu_Fon.loadFromFile("images/olympics.png");
	menuBackground.loadFromImage(imageBG);
	Sprite menu1(menuTexture1), menu2(menuTexture2), menu3(menuTexture3), menu4(menuTexture4), about(aboutTexture), menuBg(menuBackground), menuFon(Menu_Fon);
	bool isMenu = 1; // Нужно ли рисовать меню
	int menuNum = 0;
	menuFon.setPosition(0, 0);
	menu1.setPosition(30, 30);
	menu2.setPosition(30, 90);
	menu3.setPosition(30, 150);
	menu4.setPosition(30, 210);
	about.setPosition(0, 0);
	menuBg.setPosition(300, 10);

	///////////MENU/////////////
	while (isMenu) {

		menu1.setColor(Color::White);
		menu2.setColor(Color::White);
		menu3.setColor(Color::White);
		menu4.setColor(Color::White);
		menuNum = 0;
		window.clear(Color::Cyan);
		
		if (IntRect(30, 30, 160, 30).contains(Mouse::getPosition(window))) { menu1.setColor(Color::Cyan); menuNum = 1; }
		if (IntRect(30, 90, 160, 30).contains(Mouse::getPosition(window))) { menu2.setColor(Color::Cyan); menuNum = 2; }
		if (IntRect(30, 150, 160, 30).contains(Mouse::getPosition(window))) { menu3.setColor(Color::Cyan); menuNum = 3; }
		if (IntRect(30, 210, 160, 30).contains(Mouse::getPosition(window))) { menu4.setColor(Color::Cyan); menuNum = 4; }

		if (Mouse::isButtonPressed(Mouse::Left)) {
			if (menuNum == 1) {
				if (ChangeGame(window) == false) {
					menuMusic.stop();
					window.close();
					Run_Game();
				}
				isMenu = false;
			}
			if (menuNum == 2) {
				if (Setting(window) == false) {
					menuMusic.stop();
					window.close();
					Run_Game();
				}
				isMenu = false;
			}
			if (menuNum == 3) {
				window.draw(about);
				window.display();
				while (!Keyboard::isKeyPressed(Keyboard::Escape));
			}
			if (menuNum == 4) {
				window.close();
				isMenu = false;
			}
		}

		window.draw(menuFon);
		window.draw(menuBg);
		window.draw(menu1);
		window.draw(menu2);
		window.draw(menu3);
		window.draw(menu4);

		window.display();
	}
}

int main() {
	ifstream file("record_time.txt");
	file >> record_time;
	file.close();
	//std::cout << record_time;
	ifstream file1("score.txt");
	file1 >> score;
	file1.close();
	//std::cout << score;
	Run_Game();
	return 0;
}