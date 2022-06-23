# Generated by Django 4.0.5 on 2022-06-07 18:32

from django.db import migrations, models


class Migration(migrations.Migration):

    dependencies = [
        ('mycalendar', '0002_rename_calendar_id_event_calendar_and_more'),
    ]

    operations = [
        migrations.AlterField(
            model_name='event',
            name='event_type',
            field=models.CharField(choices=[('WR', 'Работа'), ('RL', 'Отдых')], default='LR', max_length=2),
        ),
    ]
