import javax.swing.*;
import java.io.*;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.util.*;
import java.util.Timer;

public class StringPool {
    EncryptString es;
    List<String> initPool;
    List<String> pool;
    Random random;

    public StringPool() {
        es = new EncryptString();
        random = new Random();
        initPool = new ArrayList<>(Arrays.asList(readFile("list.es").split(",")));
        if (Files.exists(Paths.get("pool.es"))) {
            pool = new ArrayList<>(Arrays.asList(readFile("pool.es").split(",")));
        } else {
            reset();
        }
        if (pool.get(0).equals("")) {
            pool = new ArrayList<>(initPool);
        }
        Timer timer = new Timer();
        timer.schedule(new TimerTask() {
            @Override
            public void run() {
                saveFile();
            }
        }, 0, 600000); // 每10分钟保存一次
    }

    // 获取随机字符串
    protected String getRandomString() {
        if (pool.size() == 0) {
            reset();
        }
        return pool.get(random.nextInt(pool.size()));
    }

    // 重置卡池
    protected void reset() {
        pool = new ArrayList<>(initPool);
    }

    // 删除字符串
    protected void remove(String str) {
        pool.remove(str);
    }

    // 读取文件
    private String readFile(String fileName) {
        StringBuilder contentBuilder = new StringBuilder();
        String text = null;
        try (BufferedReader reader = new BufferedReader(new FileReader(fileName))) {
            contentBuilder.append(reader.readLine());
            text = es.decrypt(contentBuilder.toString());
        } catch (Exception e) {
            JOptionPane.showMessageDialog(null, fileName + "文件读取错误，无法启动本程序。", "YuXiang Drawer", JOptionPane.ERROR_MESSAGE);
            System.exit(0);
        }
        return text;
    }

    // 保存文件
    public void saveFile() {
        try (BufferedWriter writer = new BufferedWriter(new FileWriter("pool.es"))) {
            writer.write(es.encrypt(String.join(",", pool)));
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
    }
}
