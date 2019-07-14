package vooy;

import java.io.IOException;
import java.lang.InterruptedException;
import java.util.concurrent.TimeoutException;

import vooy.MessagingUtil;;

public class App {
    public static void main(String[] args) {
        try {
            System.out.println("Hello world!");
            MessagingUtil messageUtil = new MessagingUtil();
            messageUtil.init();
            messageUtil.consumeNoQueuedMessage();
            while (true) {
                Thread.sleep(1000);
            }
        } catch (IOException io) {
            System.out.println(io.toString());
        }

        catch (InterruptedException interruptedException) {
            System.out.println(interruptedException.toString());
        } catch (TimeoutException e) {
            System.out.println(e.toString());
        }

    }
}